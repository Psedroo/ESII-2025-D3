using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RelatorioEspecifico
{
    [Key]
    public int Id { get; set; }
    
    public int NumParticipantesAtiv { get; set; }
    public decimal Receita { get; set; }
    public string Feedback { get; set; } = string.Empty;

    // Relacionamento com Relatorio
    [ForeignKey("Relatorio")]
    public int IdRelatorio { get; set; }
    
    public Relatorio? Relatorio { get; set; }
    
    public List<Evento_RelatorioEspecifico> EventoRelatoriosEspecificos { get; set; } = new();

}
