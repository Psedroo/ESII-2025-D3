using System;
using System.Collections.Generic;

namespace ESII_2025_API.Models;

public partial class Relatorio
{
    public int Id { get; set; }

    public DateOnly Data { get; set; }

    public string? Informacoes { get; set; }

    public virtual ICollection<Relatorioespecifico> Relatorioespecificos { get; set; } = new List<Relatorioespecifico>();

    public virtual ICollection<Relatoriogeral> Relatoriogerals { get; set; } = new List<Relatoriogeral>();
}
