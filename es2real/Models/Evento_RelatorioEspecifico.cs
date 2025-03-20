using System.ComponentModel.DataAnnotations.Schema;

public class Evento_RelatorioEspecifico
{
    [ForeignKey("Evento")]
    public int IdEvento { get; set; }
    public Evento Evento { get; set; } = null!;

    [ForeignKey("RelatorioEspecifico")]
    public int IdRelatorioEspecifico { get; set; }
    public RelatorioEspecifico RelatorioEspecifico { get; set; } = null!;
}

