using System.ComponentModel.DataAnnotations.Schema;

public class Bilhete_Participante
{
    [ForeignKey("Evento")]
    public int IdEvento { get; set; }
    public Evento Evento { get; set; } = null!;

    [ForeignKey("Bilhete")]
    public int IdBilhete { get; set; }
    public Bilhete Bilhete { get; set; } = null!;
}