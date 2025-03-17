using System;
using System.Collections.Generic;

namespace ESII_2025_API.Models;

public partial class Utilizador
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Contacto { get; set; }

    public DateOnly? DataNascimento { get; set; }

    public virtual ICollection<Organizador> Organizadors { get; set; } = new List<Organizador>();

    public virtual ICollection<Participante> Participantes { get; set; } = new List<Participante>();
}
