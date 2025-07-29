using System;
using System.Collections.Generic;
using GoBest.Users;

namespace GoBest.Models;

public partial class User
{
    public long Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<CompanyMaintainer> CompanyMaintainers { get; set; } = new List<CompanyMaintainer>();

    public virtual ICollection<Itinerary> Itineraries { get; set; } = new List<Itinerary>();
}
