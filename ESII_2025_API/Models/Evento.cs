using System;
using System.Collections.Generic;

namespace ESII_2025_API.Models;

public partial class Evento
{
    public int Id { get; set; }

    public string? Categoria { get; set; }

    public DateOnly Data { get; set; }

    public TimeOnly Hora { get; set; }

    public string? Local { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descricao { get; set; }

    public int? CapacidadeMax { get; set; }

    public decimal? PrecoIngresso { get; set; }

    public int? IdOrganizador { get; set; }

    public virtual Organizador? IdOrganizadorNavigation { get; set; }

    public virtual ICollection<Atividade> IdAtividades { get; set; } = new List<Atividade>();

    public virtual ICollection<Bilhete> IdBilhetes { get; set; } = new List<Bilhete>();

    public virtual ICollection<Relatorioespecifico> IdRelatorioespecificos { get; set; } = new List<Relatorioespecifico>();

    public virtual ICollection<Relatoriogeral> IdRelatoriogerals { get; set; } = new List<Relatoriogeral>();
}
