namespace GoBest.Bookings;

public sealed record CreateBookingRequest(long ItineraryId) 
{
    public int TicketCount { get; init; } = 1; // Varsayılan 1 bilet
}

public sealed record CreateBookingResponse
{
    public long BookingId { get; init; }
    public decimal TotalPrice { get; init; }
    public string Status { get; init; } = "APPROVED";
    public DateTime BookingTime { get; init; }
    public int TicketCount { get; init; } = 1; // Varsayılan 1 bilet
}

public sealed record BookingResponse
{
    public long Id { get; init; }
    public string OriginCity { get; init; } = string.Empty;
    public string DestCity { get; init; } = string.Empty;
    public decimal TotalPrice { get; init; }
    public string Status { get; init; } = "APPROVED";
    public DateTime Departure { get; init; }
    public DateTime Arrival { get; init; }
    public DateTime BookingTime { get; init; }
    public int TicketCount { get; init; }
}


