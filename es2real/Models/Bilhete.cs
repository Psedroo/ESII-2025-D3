using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public enum TipoBilhete
{
    VIP,
    Normal,
    NormalPlus // Replace "Normal+" with a valid name (no special characters)
}

public class Bilhete
{
    [Key]
    public int Id { get; set; }
    
    public TipoBilhete Tipo { get; set; } // Use enum directly, no foreign key
    
    public string Descricao { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    
    [ForeignKey("Participante")]
    public int IdParticipante { get; set; }
    public Participante? Participante { get; set; }
    
    public List<Evento_Bilhete> EventoBilhetes { get; set; } = new();
}