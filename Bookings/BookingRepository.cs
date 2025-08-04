using GoBest.Data;
using GoBest.Models;
using Microsoft.EntityFrameworkCore;

namespace GoBest.Bookings;

public class BookingRepository
{
    private readonly MyDbContext _db;
    public BookingRepository(MyDbContext db) => _db = db;

    public Task<Itinerary?> GetItineraryWithLegsAsync(long itinId, CancellationToken ct) =>
        _db.Itineraries
           .Include(i => i.ItineraryLegs)
           .FirstOrDefaultAsync(i => i.Id == itinId, ct);

    public async Task AddAsync(Booking booking, CancellationToken ct)
    {
        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Booking>> GetBookingsByUserAsync(long userId, CancellationToken ct) {
    return await _db.Bookings
       .AsNoTracking()
       .Where(b => b.UserId == userId)
       .Include(b => b.Itinerary!)
           .ThenInclude(i => i.ItineraryLegs)
               .ThenInclude(l => l.Service)
                   .ThenInclude(s => s.OriginStation).ThenInclude(st => st.City)
       .Include(b => b.Itinerary!)
           .ThenInclude(i => i.ItineraryLegs)
               .ThenInclude(l => l.Service)
                   .ThenInclude(s => s.DestStation).ThenInclude(st => st.City)
       .ToListAsync(ct);
    }
}
