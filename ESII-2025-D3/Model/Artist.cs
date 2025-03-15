using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class Artist
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Bio { get; set; }
    public DateTime BirthDate { get; set; }
}