using System;
using System.Collections.Generic;

namespace ESII_2025_API.Models;

public partial class Relatoriogeral
{
    public int Id { get; set; }

    public int? NumPorCat { get; set; }

    public string? MaisPopul { get; set; }

    public int? TotalPart { get; set; }

    public int? IdRelatorio { get; set; }

    public virtual Relatorio? IdRelatorioNavigation { get; set; }

    public virtual ICollection<Evento> IdEventos { get; set; } = new List<Evento>();
}
