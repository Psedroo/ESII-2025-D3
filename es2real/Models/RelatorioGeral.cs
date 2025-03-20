using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RelatorioGeral
{
    [Key]
    public int Id { get; set; }
    
    public int NumPorCat { get; set; }
    public string MaisPopul { get; set; } = string.Empty;
    public int TotalPart { get; set; }

    // Relacionamento com Relatorio
    [ForeignKey("Relatorio")]
    public int IdRelatorio { get; set; }
    
    public Relatorio? Relatorio { get; set; }
    
    public List<Evento_RelatorioGeral> EventoRelatoriosGerais { get; set; } = new();

}
