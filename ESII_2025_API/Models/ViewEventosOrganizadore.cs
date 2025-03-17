using System;
using System.Collections.Generic;

namespace ESII_2025_API.Models;

public partial class ViewEventosOrganizadore
{
    public int? EventoId { get; set; }

    public string? EventoNome { get; set; }

    public DateOnly? Data { get; set; }

    public TimeOnly? Hora { get; set; }

    public string? Local { get; set; }

    public decimal? PrecoIngresso { get; set; }

    public int? OrganizadorId { get; set; }

    public string? OrganizadorNome { get; set; }

    public string? Contacto { get; set; }
}
