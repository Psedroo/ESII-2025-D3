using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Evento
{
    [Key]
    public int Id { get; set; }
    
    public string Categoria { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public TimeSpan Hora { get; set; }
    public string Local { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int CapacidadeMax { get; set; }
    public decimal PrecoIngresso { get; set; }
    
    [ForeignKey("Organizador")]
    public int IdOrganizador { get; set; }
    
    public Organizador? Organizador { get; set; }
    
    public List<Evento_Atividade> EventoAtividades { get; set; } = new();
    public List<Evento_Bilhete> EventoBilhetes { get; set; } = new();
    public List<Evento_RelatorioEspecifico> EventoRelatoriosEspecificos { get; set; } = new();
    public List<Evento_RelatorioGeral> EventoRelatoriosGerais { get; set; } = new();

}
