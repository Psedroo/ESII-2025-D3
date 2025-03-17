using System;
using System.Collections.Generic;

namespace ESII_2025_API.Models;

public partial class ViewRelatoriosEvento
{
    public int? RelatorioId { get; set; }

    public DateOnly? Data { get; set; }

    public string? Informacoes { get; set; }

    public int? RelatorioEspecificoId { get; set; }

    public int? NumParticipantesAtiv { get; set; }

    public decimal? Receita { get; set; }

    public string? Feedback { get; set; }

    public int? RelatorioGeralId { get; set; }

    public int? NumPorCat { get; set; }

    public string? MaisPopul { get; set; }

    public int? TotalPart { get; set; }
}
