using GoBest.Data;
using GoBest.Models;

namespace GoBest.Routes
{
    
    public class ServiceApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ServiceRepository _serviceRepository;

        public ServiceApiService(HttpClient httpClient, ServiceRepository serviceRepository)
        {
            _httpClient = httpClient;
            _serviceRepository = serviceRepository;
        }

        public async Task FetchAndSaveAsync()
        {

            var response = await _httpClient.GetAsync("http://127.0.0.1:8000/services/mock");
        }
    }
}