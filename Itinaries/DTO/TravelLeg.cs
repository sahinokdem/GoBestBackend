using GoBest.Models;           // Service, ServiceSeatInventory, SeatType

namespace GoBest.Itineraries.DTO;

/// <summary>Seçilen koltuk tipini de içeren tek bacak.</summary>
public sealed record TravelLeg(Service Service, ServiceSeatInventory Inv)
{
    public SeatType SeatType => Inv.SeatType!;
    public decimal  Price    => Inv.Price;
}
