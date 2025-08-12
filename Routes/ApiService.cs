using GoBest.Companies;
using GoBest.Data;
using GoBest.Models;
using GoBest.Routes.DTO;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json; // Add this using statement

namespace GoBest.Routes
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly RouteService _routeService;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, ILogger<ApiService> logger, RouteService routeService)
        {
            _httpClient = httpClient;
            _routeService = routeService;
            _logger = logger;
        }

        public async Task<List<ServiceAPIDto>> GetApiResponseAsync(string url)
        {
            // HttpClient DI üzerinden geldiği için tekrar new yapma
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<ServiceAPIDto>>();

            if (result == null)
                throw new Exception("API response was null or invalid");

            return result;
        }

        public async Task StartHourlyRequestsAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Sending API request...");

                    var services = await GetApiResponseAsync("http://127.0.0.1:8000/services/mock");
                    _logger.LogInformation("Fetched {Count} services", services.Count);

                    foreach (ServiceAPIDto s in services)
                    {
                        _routeService.SaveRouteFromApi(s).Wait(stoppingToken);
                        
                        string jsonString = JsonSerializer.Serialize(s, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });
                        _logger.LogInformation($"Service JSON:\n{jsonString}\n");
                        
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching services");
                }

                await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
            }
        }
    }
}
