using System.ComponentModel.DataAnnotations.Schema;
using ES2Real.Models;

public class AtividadeParticipante
{
    [ForeignKey("Atividade")]
    public int IdAtividade { get; set; }

    [ForeignKey("Participante")]
    public int IdParticipante { get; set; }

    public Atividade Atividade { get; set; } = null!;
    public Participante Participante { get; set; } = null!;
}