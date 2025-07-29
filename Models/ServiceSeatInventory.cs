﻿using System;
using System.Collections.Generic;

namespace GoBest.Models;

public partial class ServiceSeatInventory
{
    public long Id { get; set; }

    public long? ServiceId { get; set; }

    public long? SeatTypeId { get; set; }

    public int Capacity { get; set; }

    public int Available { get; set; }

    public decimal Price { get; set; }

    public virtual SeatType? SeatType { get; set; }

    public virtual Service? Service { get; set; }
}
