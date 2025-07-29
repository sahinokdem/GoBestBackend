using System;
using System.Collections.Generic;

namespace GoBest.Models;

public partial class ItineraryLeg
{
    public long Id { get; set; }

    public long? ItineraryId { get; set; }

    public short LegOrder { get; set; }

    public long? ServiceId { get; set; }

    public long? SeatTypeId { get; set; }

    public decimal? Price { get; set; }

    public virtual Itinerary? Itinerary { get; set; }

    public virtual SeatType? SeatType { get; set; }

    public virtual Service? Service { get; set; }
}
