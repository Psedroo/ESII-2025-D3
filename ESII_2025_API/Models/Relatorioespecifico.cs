using System;
using System.Collections.Generic;

namespace ESII_2025_API.Models;

public partial class Relatorioespecifico
{
    public int Id { get; set; }

    public int? NumParticipantesAtiv { get; set; }

    public decimal? Receita { get; set; }

    public string? Feedback { get; set; }

    public int? IdRelatorio { get; set; }

    public virtual Relatorio? IdRelatorioNavigation { get; set; }

    public virtual ICollection<Evento> IdEventos { get; set; } = new List<Evento>();
}
