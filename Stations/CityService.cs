using GoBest.Models;
using GoBest.Routes.DTO;

namespace GoBest.Stations;

public class CityService
{
    private readonly CityRepository _cityRepository;

    public CityService(CityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<long> SaveCityFromApi(CityAPIDto apiDto)
    {
        if (apiDto == null)
        {
            throw new ArgumentNullException(nameof(apiDto));
        }

        City city = CityMapper.ToCity(apiDto);
        return await _cityRepository.SaveAndGetCityId(city);
    }

    public async Task<List<CityResponse>> GetAllCitiesAsync()
    {
        var cities = await _cityRepository.GetAllCitiesAsync();
        var cityResponses = new List<CityResponse>();
        foreach (var city in cities)
        {
            cityResponses.Add(CityMapper.ToCityResponse(city));
        }
        return cityResponses;
    }

    public async Task<bool> UpdateCityAsync(long id, UpdateCityRequest dto)
    {
        var city = await _cityRepository.GetCityByIdAsync(id);
        if (city is null) return false;

        city.Name        = dto.Name.Trim();
        city.CountryCode = dto.CountryCode.ToUpperInvariant();

        await _cityRepository.SaveCityAsync(city);
        return true;
    }
}