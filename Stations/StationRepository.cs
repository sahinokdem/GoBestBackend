using GoBest.Data;
using GoBest.Models;
using GoBest.Routes.DTO;
using Microsoft.EntityFrameworkCore;

namespace GoBest.Stations
{
    public class StationRepository
    {
        private readonly MyDbContext _db;

        public StationRepository(MyDbContext db)
        {
            _db = db;
        }

        public async Task<Station> SaveStationFromApiAsync(StationAPIDto stationDto)
        {
            // First ensure city exists
            var city = await SaveCityFromApiAsync(stationDto.City);
            
            var station = await _db.Stations
                .Where(s => s.Code == stationDto.Code || s.Name == stationDto.Name)
                .FirstOrDefaultAsync();
                
            if (station == null)
            {
                station = new Station
                {
                    Name = stationDto.Name,
                    Code = stationDto.Code,
                    CityId = city.Id,
                    Latitude = stationDto.Latitude,
                    Longitude = stationDto.Longitude
                };
                
                _db.Stations.Add(station);
                await _db.SaveChangesAsync();
            }
            else
            {
                // Update existing station with latest info
                station.Name = stationDto.Name;
                station.Code = stationDto.Code;
                station.CityId = city.Id;
                station.Latitude = stationDto.Latitude;
                station.Longitude = stationDto.Longitude;
                
                _db.Stations.Update(station);
                await _db.SaveChangesAsync();
            }
            
            return station;
        }
        
        private async Task<City> SaveCityFromApiAsync(CityAPIDto cityDto)
        {
            var city = await _db.Cities
                .Where(c => c.Name == cityDto.Name && c.CountryCode == cityDto.Country_Code)
                .FirstOrDefaultAsync();
                
            if (city == null)
            {
                city = new City
                {
                    Name = cityDto.Name,
                    CountryCode = cityDto.Country_Code
                };
                
                _db.Cities.Add(city);
                await _db.SaveChangesAsync();
            }
            
            return city;
        }
    }
}
