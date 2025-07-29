using System;
using System.Collections.Generic;

namespace GoBest.Models;

public partial class City
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public virtual ICollection<Itinerary> ItineraryDestCities { get; set; } = new List<Itinerary>();

    public virtual ICollection<Itinerary> ItineraryOriginCities { get; set; } = new List<Itinerary>();

    public virtual ICollection<Station> Stations { get; set; } = new List<Station>();
}
