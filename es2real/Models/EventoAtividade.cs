using System.ComponentModel.DataAnnotations.Schema;

public class Evento_Atividade
{
    [ForeignKey("Evento")]
    public int IdEvento { get; set; }
    public Evento Evento { get; set; } = null!;

    [ForeignKey("Atividade")]
    public int IdAtividade { get; set; }
    public Atividade Atividade { get; set; } = null!;
}