using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoBest.Stations;

[ApiController]
[Route("api/[controller]")]
public class CityController : ControllerBase
{
    private readonly CityService _cityService;

    public CityController(CityService cityService)
    {
        _cityService = cityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCities()
    {
        var cities = await _cityService.GetAllCitiesAsync();
        return Ok(cities);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateCity(long id, [FromBody] UpdateCityRequest dto)
        => await _cityService.UpdateCityAsync(id, dto) ? NoContent() : NotFound();
}