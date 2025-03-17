using System;
using System.Collections.Generic;

namespace ESII_2025_API.Models;

public partial class Organizador
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Contacto { get; set; }

    public DateOnly? DataNascimento { get; set; }

    public int? IdUtilizador { get; set; }

    public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();

    public virtual Utilizador? IdUtilizadorNavigation { get; set; }
}
