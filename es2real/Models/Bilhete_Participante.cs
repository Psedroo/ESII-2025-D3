using System.ComponentModel.DataAnnotations.Schema;
using ES2Real.Models;

public class Bilhete_Participante
{
    [ForeignKey("Participante")]
    public int IdParticipante { get; set; }
    public Participante Participante { get; set; } = null!;

    [ForeignKey("Bilhete")]
    public int IdBilhete { get; set; }
    public Bilhete Bilhete { get; set; } = null!;
}