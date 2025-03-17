using System;
using System.Collections.Generic;

namespace ESII_2025_API.Models;

public partial class ViewParticipantesBilhete
{
    public int? ParticipanteId { get; set; }

    public string? ParticipanteNome { get; set; }

    public string? Contacto { get; set; }

    public int? BilheteId { get; set; }

    public string? Tipo { get; set; }

    public string? Descricao { get; set; }

    public int? Quantidade { get; set; }
}
