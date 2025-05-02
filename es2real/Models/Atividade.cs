using System.ComponentModel.DataAnnotations;

public class Atividade
{
    [Key]
    public int Id { get; set; }
    
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public TimeSpan Hora { get; set; }
    
    public List<Evento_Atividade> EventoAtividades { get; set; } = new();

}