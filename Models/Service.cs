﻿using System;
using System.Collections.Generic;

namespace GoBest.Models;

public partial class Service
{
    public long Id { get; set; }

    public long? CompanyId { get; set; }

    public long? OriginStationId { get; set; }

    public long? DestStationId { get; set; }

    public string ServiceCode { get; set; } = null!;

    public DateTime DepartureTime { get; set; }

    public DateTime ArrivalTime { get; set; }

    public decimal BasePrice { get; set; }

    public bool? Sold { get; set; }

    public int? SalesCount { get; set; }

    public virtual ICollection<BookingLeg> BookingLegs { get; set; } = new List<BookingLeg>();

    public virtual Company? Company { get; set; }

    public virtual Station? DestStation { get; set; }

    public virtual ICollection<ItineraryLeg> ItineraryLegs { get; set; } = new List<ItineraryLeg>();

    public virtual Station? OriginStation { get; set; }

    public virtual ICollection<ServiceSeatInventory> ServiceSeatInventories { get; set; } = new List<ServiceSeatInventory>();
}
