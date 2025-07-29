using System;
using System.Collections.Generic;

namespace GoBest.Models;

public partial class BookingLeg
{
    public long Id { get; set; }

    public long? BookingId { get; set; }

    public long? ServiceId { get; set; }

    public long? SeatTypeId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual SeatType? SeatType { get; set; }

    public virtual Service? Service { get; set; }
}
