using GoBest.Data;
using GoBest.Models;
using Microsoft.EntityFrameworkCore;

namespace GoBest.Seats;

public class SeatRepository
{
    private readonly MyDbContext _db;

    public SeatRepository(MyDbContext db)
    {
        _db = db;
    }

    public async Task addSeatTypeAsync(SeatType seat)
    {
        if (seat == null) throw new ArgumentNullException(nameof(seat));
        _db.SeatTypes.Add(seat);
        await _db.SaveChangesAsync();
    }

    public async Task AddOrGetSeatTypesInOrderAsync(List<SeatType> seats)
    {
        if (seats is null || seats.Count == 0) throw new ArgumentNullException(nameof(seats));

        var result = new List<SeatType>(seats.Count);
        var toAddDb = new List<SeatType>();

        foreach (var st in seats)
        {
            var existing = await _db.SeatTypes
                .FirstOrDefaultAsync(x => x.Name == st.Name && x.Mode == st.Mode);

            if (existing is null)
            {
                toAddDb.Add(st);
                result.Add(st);
            }
            else
            {
                result.Add(existing);
            }
        }

        _db.SeatTypes.AddRange(toAddDb);
        await _db.SaveChangesAsync();

        // orijinal sırayı korumak için sonucu geri döndür
        seats.Clear();
        seats.AddRange(result);
    }


    public async Task AddSeatInventoriesAsync(List<ServiceSeatInventory> inventories)
    {
        if (inventories == null || !inventories.Any()) throw new ArgumentNullException(nameof(inventories));
        _db.ServiceSeatInventories.AddRange(inventories);
        await _db.SaveChangesAsync();
    }

    public async Task SetServiceSeatInventoryCapacityAndAvailableAsync(long inventoryId, int capacity, int available)
    {
        if (capacity < 0 || available < 0) throw new ArgumentOutOfRangeException("Capacity and available seats must be non-negative.");
        
        var inventory = await _db.ServiceSeatInventories
            .FirstOrDefaultAsync(i => i.Id == inventoryId);
        
        if (inventory == null)
        {
            throw new InvalidOperationException("ServiceSeatInventory not found for the given service and seat type.");
        }

        inventory.Capacity = capacity;
        inventory.Available = available;

        await _db.SaveChangesAsync();
    }
}