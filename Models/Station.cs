using System;
using System.Collections.Generic;

namespace GoBest.Models;

public partial class Station
{
    public long Id { get; set; }

    public long? CityId { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? StationType { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public virtual City? City { get; set; }

    public virtual ICollection<Service> ServiceDestStations { get; set; } = new List<Service>();

    public virtual ICollection<Service> ServiceOriginStations { get; set; } = new List<Service>();
}
