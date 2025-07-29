using System;
using System.Collections.Generic;
using GoBest.Companies;

namespace GoBest.Models;

public partial class Company
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public CompanyMode Mode { get; set; }

    public string CountryCode { get; set; } = null!;

    public string? IataCode { get; set; }

    public virtual ICollection<CompanyMaintainer> CompanyMaintainers { get; set; } = new List<CompanyMaintainer>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
