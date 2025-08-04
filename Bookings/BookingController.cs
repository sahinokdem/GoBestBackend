using System.Security.Claims;
using GoBest.Bookings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoBest.Bookings;

[ApiController, Route("api/bookings")]
[Authorize(Policy = "CustomerOnly")]
public sealed class BookingController : ControllerBase
{
    private readonly BookingService _svc;
    public BookingController(BookingService svc) => _svc = svc;

    [HttpPost]
    public async Task<ActionResult<CreateBookingResponse>> Create(
        [FromBody] CreateBookingRequest req,
        CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null) return Unauthorized();
        long userId = long.Parse(userIdClaim.Value);


        var res = await _svc.BookAsync(req, userId, ct);
        return res is null ? NotFound("Itinerary not found") : Ok(res);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookingResponse>>> MyBookings(CancellationToken ct)
    {
        var uidClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (uidClaim is null) return Unauthorized();

        long userId = long.Parse(uidClaim.Value);

        var rows = await _svc.GetMyBookingsAsync(userId, ct);
        return Ok(rows);
    }
}
