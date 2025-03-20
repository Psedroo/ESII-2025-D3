using System.ComponentModel.DataAnnotations.Schema;

public class Evento_RelatorioGeral
{
    [ForeignKey("Evento")]
    public int IdEvento { get; set; }
    public Evento Evento { get; set; } = null!;

    [ForeignKey("RelatorioGeral")]
    public int IdRelatorioGeral { get; set; }
    public RelatorioGeral RelatorioGeral { get; set; } = null!;
}

