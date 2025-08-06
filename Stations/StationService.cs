namespace GoBest.Stations
{
    using GoBest.Models;
    using GoBest.Routes.DTO;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class StationService
    {
        private readonly StationRepository _stationRepository;
        private readonly CityService _cityService;

        public StationService(StationRepository stationRepository, CityService cityService)
        {
            _stationRepository = stationRepository;
            _cityService = cityService;
        }

        public async Task<List<StationResponse>> GetAllStationsAsync()
        {
            var stations = await _stationRepository.GetAllStationsAsync();
            var stationResponses = new List<StationResponse>();
            foreach (var station in stations)
            {
                stationResponses.Add(StationMapper.ToStationResponse(station));
            }
            return stationResponses;
        }

        public async Task<bool> UpdateStationAsync(long id, UpdateStationRequest dto)
        {
            var station = await _stationRepository.GetStationByIdAsync(id);
            if (station is null) return false;

            station.Name = dto.Name.Trim();
            station.Code = dto.Code.Trim();

            await _stationRepository.SaveStationAsync(station);
            return true;
        }


        public async Task<long> SaveOriginFromApi(ServiceAPIDto apiDto)
        {
            if (apiDto == null)
            {
                throw new ArgumentNullException(nameof(apiDto));
            }
            long origin_city_id = _cityService.SaveCityFromApi(apiDto.Origin.City).Result;
            Station origin = StationMapper.ToOrigin(apiDto, origin_city_id);
            long stationId = await _stationRepository.SaveAndGetStationId(origin);
            return stationId;
        }

        public async Task<long> SaveDestinationFromApi(ServiceAPIDto apiDto)
        {
            if (apiDto == null)
            {
                throw new ArgumentNullException(nameof(apiDto));
            }
            long destination_city_id = _cityService.SaveCityFromApi(apiDto.Destination.City).Result;
            Station destination = StationMapper.ToDest(apiDto, destination_city_id);
            long stationId = await _stationRepository.SaveAndGetStationId(destination);
            return stationId;
        }

    }
    
}