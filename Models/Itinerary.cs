using System;
using System.Collections.Generic;

namespace GoBest.Models;

public partial class Itinerary
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public long? OriginCityId { get; set; }

    public long? DestCityId { get; set; }

    public DateTime SearchTime { get; set; }

    public decimal? TotalPrice { get; set; }

    public TimeSpan? TotalDuration { get; set; }

    public short? TotalLegs { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual City? DestCity { get; set; }

    public virtual ICollection<ItineraryLeg> ItineraryLegs { get; set; } = new List<ItineraryLeg>();

    public virtual City? OriginCity { get; set; }

    public virtual User? User { get; set; }
}
