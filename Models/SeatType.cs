using System;
using System.Collections.Generic;
using GoBest.Companies;

namespace GoBest.Models;

public partial class SeatType
{
    public long Id { get; set; }

    public CompanyMode Mode { get; set; }

    public string Name { get; set; } = null!;

    public decimal PriceMultiplier { get; set; }

    public virtual ICollection<BookingLeg> BookingLegs { get; set; } = new List<BookingLeg>();

    public virtual ICollection<ItineraryLeg> ItineraryLegs { get; set; } = new List<ItineraryLeg>();

    public virtual ICollection<ServiceSeatInventory> ServiceSeatInventories { get; set; } = new List<ServiceSeatInventory>();
}
