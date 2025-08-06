using GoBest.Models;
using GoBest.Routes.DTO;

namespace GoBest.Stations
{

    public class StationMapper
    {
        public static Station ToOrigin(ServiceAPIDto apiDto, long cityId)
        {
            if (apiDto == null)
            {
                throw new ArgumentNullException(nameof(apiDto));
            }

            StationAPIDto stationDto = apiDto.Origin;
            return new Station
            {
                Name = stationDto.Name,
                Latitude = Convert.ToDecimal(stationDto.Latitude),
                Longitude = Convert.ToDecimal(stationDto.Longitude),
                Code = stationDto.Code,
                StationType = apiDto.Mode,
                CityId = cityId
            };
        }

        public static Station ToDest(ServiceAPIDto apiDto, long cityId)
        {
            if (apiDto == null)
            {
                throw new ArgumentNullException(nameof(apiDto));
            }

            StationAPIDto stationDto = apiDto.Destination;
            return new Station
            {
                Name = stationDto.Name,
                Latitude = Convert.ToDecimal(stationDto.Latitude),
                Longitude = Convert.ToDecimal(stationDto.Longitude),
                Code = stationDto.Code,
                StationType = apiDto.Mode,
                CityId = cityId
            };
        }

        internal static StationResponse ToStationResponse(Station station)
        {
            if (station == null)
            {
                throw new ArgumentNullException(nameof(station));
            }

            return new StationResponse
            {
                Id = station.Id,
                Name = station.Name,
                Code = station.Code,
                CityId = station.CityId,
                StationType = station.StationType,
                Latitude = station.Latitude,
                Longitude = station.Longitude
            };
        }
    }
}