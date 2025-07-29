using System;
using System.Collections.Generic;

namespace GoBest.Models;

public partial class CompanyMaintainer
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public long? CompanyId { get; set; }

    public virtual Company? Company { get; set; }

    public virtual User? User { get; set; }
}
