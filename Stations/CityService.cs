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
}