﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Participante
{
    [Key]
    public int Id { get; set; }
    
    public string Nome { get; set; } = string.Empty;
    public string Contacto { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    
    [ForeignKey("UtilizadorAuth")]
    public int IdUtilizador { get; set; }
    
    public UtilizadorAuth? Utilizador { get; set; }
    
    public List<Bilhete_Participante> BilheteParticipante { get; set; } = new();
}
