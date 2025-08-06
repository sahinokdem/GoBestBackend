using GoBest.Companies;
using GoBest.Models;
using GoBest.Routes.DTO;
using GoBest.Seats;
using GoBest.Stations;

namespace GoBest.Routes
{
    public class RouteService
    {
        private readonly ServiceRepository _serviceRepository;
        private readonly CompanyService _companyService;
        private readonly StationService _stationService;
        private readonly SeatInventoryService _seatInventoryService;
        private readonly CompanyMaintainerService _companyMaintainerService;

        private readonly ILogger<RouteService> _logger;
        public RouteService(ServiceRepository serviceRepository, CompanyService companyService,
         StationService stationService, SeatInventoryService seatInventoryService, ILogger<RouteService> logger, CompanyMaintainerService companyMaintainerService)
        {
            _serviceRepository = serviceRepository;
            _companyService = companyService;
            _stationService = stationService;
            _seatInventoryService = seatInventoryService;
            _logger = logger;
            _companyMaintainerService = companyMaintainerService;
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

            // Create and save seat inventory
            await _seatInventoryService.AddSeatInventoryFromApi(apiDto.Seat_Types, service.Id, CompanyMapper.ToCompanyMode(apiDto));
            _logger.LogInformation("Added seat inventory for service with ID: {ServiceId}", service.Id);
        }

        public async Task<bool> UpdateServiceAsync(long userId, long serviceId, UpdateServiceRequest dto)
        {
            var companyId = await _companyMaintainerService.GetCompanyIdByMaintainerAsync(userId);
            if (companyId == 0)
            {
                throw new UnauthorizedAccessException("User is not a maintainer of any company.");
            }
            var service = await _serviceRepository.GetServiceByCompanyIdAsync(companyId, serviceId);
            if (service is null) return false;
            service.DepartureTime = dto.DepartureTime;
            service.ArrivalTime = dto.ArrivalTime;

            await _serviceRepository.SaveAsync(service);
            return true;
        }

        internal async Task<IReadOnlyList<Service>> GetAllServicesAsync()
        {
            return await _serviceRepository.GetAllServicesAsync();
        }

        public async Task<IReadOnlyList<ServiceResponse>> GetAllServicesOfCompanyAsync(long userId)
        {
            var companyId = await _companyMaintainerService.GetCompanyIdByMaintainerAsync(userId);

            if (companyId == 0)
                throw new UnauthorizedAccessException("Invalid maintainer");

            var services = await _serviceRepository.GetServicesByCompanyIdAsync(companyId);
            return ServiceMapper.ToResponses(services);
        }

    }
}