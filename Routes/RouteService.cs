using GoBest.Companies;
using GoBest.Routes.DTO;
using GoBest.Stations;

namespace GoBest.Routes
{
    public class RouteService
    {
        private readonly ServiceRepository _serviceRepository;
        private readonly CompanyService _companyService;
        private readonly StationService _stationService;

        public RouteService(ServiceRepository serviceRepository, CompanyService companyService, StationService stationService)
        {
            _serviceRepository = serviceRepository;
            _companyService = companyService;
            _stationService = stationService;
        }

        public async Task SaveRouteFromApi(ServiceAPIDto apiDto)
        {
            /*
            if (apiDto == null)
            {
                throw new ArgumentNullException(nameof(apiDto));
            }

            // Save company
            long companyId = await _companyService.saveCompanyFromApi(apiDto.Company);

            // Save stations
            foreach (var stationDto in apiDto.Stations)
            {
                Station station = StationMapper.ToStation(stationDto);
                await _stationService.SaveStationAsync(station);
            }

            // Save route
            await _serviceRepository.SaveFromApi(apiDto, companyId);
            */
        }
    }
}