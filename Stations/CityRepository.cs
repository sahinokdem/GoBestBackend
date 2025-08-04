using GoBest.Data;
using GoBest.Models;
using Microsoft.EntityFrameworkCore;

namespace GoBest.Stations;

public class CityRepository
{
    private readonly MyDbContext _db;

    public CityRepository(MyDbContext db)
    {
        _db = db;
    }

    public async Task<City?> GetCityByIdAsync(long cityId)
    {
        return await _db.Cities.FindAsync(cityId);
    }

    public async Task SaveCityAsync(City city)
    {
        if (city == null) throw new ArgumentNullException(nameof(city));
        _db.Cities.Add(city);
        await _db.SaveChangesAsync();
    }

    public async Task<long> SaveAndGetCityId(City city)
    {
        if (city == null) throw new ArgumentNullException(nameof(city));

        var existingCity = _db.Cities
            .Where(c => c.Name == city.Name && c.CountryCode == city.CountryCode)
            .FirstOrDefault();

        if (existingCity != null)
        {
            return existingCity.Id;
        }

        _db.Cities.Add(city);
        await _db.SaveChangesAsync();

        return city.Id;
    }
    
    public async Task<List<City>> GetAllCitiesAsync()
    {
        return await _db.Cities.ToListAsync();
    }
}