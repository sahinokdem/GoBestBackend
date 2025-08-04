using GoBest.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GoBest.Itinaries;

[ApiController]
[Route("api/routes")]
public class ItinaryController : ControllerBase
{
    private readonly ItineraryService _service;
    public ItinaryController(ItineraryService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<IReadOnlyList<SearchResponse>>> SearchAsync([FromBody] SearchRequest req)
    {
        var result = await _service.SearchAsync(req, HttpContext.RequestAborted);
        return result.Count == 0 ? NotFound(BusinessException.RouteNotFound().Message) : Ok(result);
    }
}