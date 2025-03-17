using System;
using System.Collections.Generic;

namespace ESII_2025_API.Models;

public partial class Tipobilhete
{
    public int Id { get; set; }

    public bool? Vip { get; set; }

    public bool? Normal { get; set; }

    public int? IdBilhete { get; set; }

    public virtual Bilhete? IdBilheteNavigation { get; set; }
}
