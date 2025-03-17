using System;
using System.Collections.Generic;

namespace ESII_2025_API.Models;

public partial class Atividade
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descricao { get; set; }

    public virtual ICollection<Evento> IdEventos { get; set; } = new List<Evento>();
}
