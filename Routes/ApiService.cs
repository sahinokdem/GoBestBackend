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
        private readonly CompanyService _companyService;
        private readonly ServiceRepository _serviceRepository;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, CompanyService companyService, ServiceRepository serviceRepository, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _companyService = companyService;
            _serviceRepository = serviceRepository;
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

                    var services = await GetApiResponseAsync("https://example.com/api");
                    _logger.LogInformation("Fetched {Count} services", services.Count);

                    foreach (ServiceAPIDto s in services)
                    {
                        string jsonString = JsonSerializer.Serialize(s, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });
                        Console.WriteLine($"Service JSON:\n{jsonString}\n");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching services");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
