using GoBest.Companies;
using GoBest.Data;
using GoBest.Models;
using GoBest.Routes.DTO;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace GoBest.Routes
{
    public class ServiceApiService
    {
        private readonly HttpClient _httpClient;
        private readonly CompanyService _companyService;
        private readonly ServiceRepository _serviceRepository;
        private readonly ILogger<ServiceApiService> _logger;

        public ServiceApiService(HttpClient httpClient, CompanyService companyService, ServiceRepository serviceRepository, ILogger<ServiceApiService> logger)
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

                    // Örnek DB kaydetme (ServiceRepository içinde SaveAsync gibi bir metot olmalı)
                    foreach (ServiceAPIDto s in services)
                    {
                        await _companyService.saveCompanyFromApi(s);
                        _logger.LogInformation("Company saved: {Name} ({Country})", s.Company.Name, s.Company.Country_Code);
                        await _serviceRepository.SaveFromApi(s);
                        _logger.LogInformation("Service saved: {Code} ({Mode})", s.Service_Code, s.Mode);
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
