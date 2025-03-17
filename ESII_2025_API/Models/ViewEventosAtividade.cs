using System;
using System.Collections.Generic;

namespace ESII_2025_API.Models;

public partial class ViewEventosAtividade
{
    public int? EventoId { get; set; }

    public string? EventoNome { get; set; }

    public int? AtividadeId { get; set; }

    public string? AtividadeNome { get; set; }

    public string? Descricao { get; set; }
}
