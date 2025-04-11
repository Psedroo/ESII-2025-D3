using System.ComponentModel.DataAnnotations;

public class UtilizadorAuth
{
    [Key]
    public int Id { get; set; }
    
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string SenhaSalt { get; set; }  // Store a salt if using manual hashing
    public string TipoUsuario { get; set; } = string.Empty;
}
