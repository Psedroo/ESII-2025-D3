﻿using System.ComponentModel.DataAnnotations;
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
    
    public TipoBilhete Tipo { get; set; }
    
    public string Descricao { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    
    public decimal Preco { get; set; }
    
    [ForeignKey("Evento")]
    public int idEvento { get; set; }

    public Evento Evento { get; set; } = null!;
    
    public List<Bilhete_Participante> BilheteParticipante {get; set;} = new();
}