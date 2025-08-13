
using GoBest.Models;

namespace GoBest.Bookings;

public class BookingService
{
    private readonly BookingRepository _repo;
    private readonly ILogger<BookingService> _log;

    public BookingService(BookingRepository repo, ILogger<BookingService> log)
    {
        _repo = repo; _log = log;
    }

    public async Task<CreateBookingResponse?> BookAsync(
        CreateBookingRequest req, long userId, CancellationToken ct)
    {
        var itin = await _repo.GetItineraryWithLegsAsync(req.ItineraryId, ct);
        if (itin is null) return null;

        var total = itin.ItineraryLegs.Sum(l => l.Price ?? 0);

        var booking = new Booking
        {
            UserId = userId,
            ItineraryId = itin.Id,
            BookingTime = DateTime.UtcNow,
            TotalPrice = total * req.TicketCount, // Toplam fiyatı bilet sayısıyla çarp
            Status = "APPROVED",
            TicketCount = req.TicketCount
        };

        await _repo.AddAsync(booking, ct);

        return new CreateBookingResponse
        {
            BookingId = booking.Id,
            TotalPrice = total * req.TicketCount,
            BookingTime = booking.BookingTime,
            TicketCount = booking.TicketCount, // Bilet sayısını da döndür
        };
    }
    
    public async Task<IReadOnlyList<BookingResponse>> GetMyBookingsAsync(long userId, CancellationToken ct)
    {
        var list = await _repo.GetBookingsByUserAsync(userId, ct);

        return list.Select(b =>
        {
            var legs = b.Itinerary!.ItineraryLegs
                          .OrderBy(l => l.LegOrder)
                          .Select(l => l.Service)
                          .ToArray();

            return new BookingResponse
            {
                Id = b.Id,
                OriginCity = legs.First().OriginStation!.City!.Name,
                DestCity = legs.Last().DestStation!.City!.Name,
                TotalPrice = b.TotalPrice,
                Status = b.Status,
                BookingTime = b.BookingTime,
                Departure = legs.First().DepartureTime.ToLocalTime(),
                Arrival = legs.Last().ArrivalTime.ToLocalTime(),
                TicketCount = b.TicketCount
            };
        }).ToList();
    }
}
