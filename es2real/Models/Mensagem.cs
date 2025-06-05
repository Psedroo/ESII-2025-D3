using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Mensagem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Texto { get; set; } = string.Empty;

    [ForeignKey("Evento")]
    public int IdEvento { get; set; }

    public Evento? Evento { get; set; }
}