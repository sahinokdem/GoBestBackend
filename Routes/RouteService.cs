using GoBest.Companies;
using GoBest.Models;
using GoBest.Routes.DTO;
using GoBest.Stations;

namespace GoBest.Routes
{
    public class RouteService
    {
        private readonly ServiceRepository _serviceRepository;
        private readonly CompanyService _companyService;
        private readonly StationService _stationService;

        private readonly ILogger<RouteService> _logger;        
        public RouteService(ServiceRepository serviceRepository, CompanyService companyService, StationService stationService, ILogger<RouteService> logger)
        {
            _serviceRepository = serviceRepository;
            _companyService = companyService;
            _stationService = stationService;
            _logger = logger;
        }

        public async Task SaveRouteFromApi(ServiceAPIDto apiDto)
        {
            if (apiDto == null)
            {
                throw new ArgumentNullException(nameof(apiDto));
            }
            // Save company
            long companyId = await _companyService.SaveCompanyFromApi(apiDto);
            _logger.LogInformation("Saved company with ID: {CompanyId}", companyId);

            // Save origin station
            long originStationId = await _stationService.SaveOriginFromApi(apiDto);
            _logger.LogInformation("Saved origin station with ID: {OriginStationId}", originStationId);

            // Save destination station
            long destinationStationId = await _stationService.SaveDestinationFromApi(apiDto);
            _logger.LogInformation("Saved destination station with ID: {DestinationStationId}", destinationStationId);

            // Create and save service
            Service service = ServiceMapper.ToService(apiDto, companyId, originStationId, destinationStationId);
            await _serviceRepository.SaveAsync(service);
            _logger.LogInformation("Saved service with ID: {ServiceId}", service.Id);
        }
    }
}