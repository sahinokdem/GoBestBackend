using System.Security.Claims;
using GoBest.Routes.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoBest.Routes;


public class ServiceController : ControllerBase
{
    private readonly RouteService _routeService;

    public ServiceController(RouteService routeService)
    {
        _routeService = routeService;
    }

    [HttpGet("services")]
    public async Task<IActionResult> GetAllServices()
    {
        var services = await _routeService.GetAllServicesAsync();
        return Ok(services);
    }

    [Authorize(Policy = "CompanyRepOnly")]
    [HttpPut("services/{serviceId:long}")]
    public async Task<IActionResult> UpdateService(long serviceId, [FromBody] UpdateServiceRequest dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null) return Unauthorized();
        long userId = long.Parse(userIdClaim.Value);
        var updated = await _routeService.UpdateServiceAsync(userId, serviceId, dto);
        return updated ? NoContent() : NotFound();
    }

    [Authorize(Policy = "CompanyRepOnly")]
    [HttpGet("services/company")]
    public async Task<IActionResult> GetAllServicesOfCompany()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null) return Unauthorized();
        long userId = long.Parse(userIdClaim.Value);
        var services = await _routeService.GetAllServicesOfCompanyAsync(userId);
        return Ok(services);
    }
}