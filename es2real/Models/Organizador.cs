using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Organizador
{
    [Key]
    public int Id { get; set; }
    
    public string Nome { get; set; } = string.Empty;
    public string Contacto { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    
    [ForeignKey("Usuario")]
    public int IdUsuario { get; set; }
    
    public UtilizadorAuth? Usuario { get; set; }
}
