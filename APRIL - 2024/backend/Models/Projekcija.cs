using System.Text.Json.Serialization;

namespace backend.Models;

public class Projekcija{
    [Key]
    public int ID {get; set;}
    public required string Naziv {get; set;}
    public DateTime VremePrikazivanja {get; set;}
    public List<Karta>? Karte {get; set;}
}