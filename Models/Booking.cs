using System;
using System.Collections.Generic;

namespace GoBest.Models;

public partial class Booking
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public long? ItineraryId { get; set; }

    public DateTime BookingTime { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = null!;

    public int TicketCount { get; set; } = 1;   // ★ YENİ

    public virtual ICollection<BookingLeg> BookingLegs { get; set; } = new List<BookingLeg>();

    public virtual Itinerary? Itinerary { get; set; }

    public virtual User? User { get; set; }
}
