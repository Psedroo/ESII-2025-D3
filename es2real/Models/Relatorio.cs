using System.ComponentModel.DataAnnotations;

public class Relatorio
{
    [Key]
    public int Id { get; set; }
    
    public DateTime Data { get; set; }
    
    public string? Informacoes { get; set; } 

}
