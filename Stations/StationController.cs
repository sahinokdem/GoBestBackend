using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoBest.Stations;

[ApiController]
[Route("api/[controller]")]
public class StationController : ControllerBase
{
    private readonly StationService _stationService;

    public StationController(StationService stationService)
    {
        _stationService = stationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStations()
    {
        var stations = await _stationService.GetAllStationsAsync();
        return Ok(stations);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateStation(long id, [FromBody] UpdateStationRequest dto)
        => await _stationService.UpdateStationAsync(id, dto) ? NoContent() : NotFound();
}