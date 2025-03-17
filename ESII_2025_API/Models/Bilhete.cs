using System;
using System.Collections.Generic;

namespace ESII_2025_API.Models;

public partial class Bilhete
{
    public int Id { get; set; }

    public string? Tipo { get; set; }

    public string? Descricao { get; set; }

    public int? Quantidade { get; set; }

    public int? IdParticipante { get; set; }

    public virtual Participante? IdParticipanteNavigation { get; set; }

    public virtual ICollection<Tipobilhete> Tipobilhetes { get; set; } = new List<Tipobilhete>();

    public virtual ICollection<Evento> IdEventos { get; set; } = new List<Evento>();
}
