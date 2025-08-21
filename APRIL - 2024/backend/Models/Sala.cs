using System.Text.Json.Serialization;

namespace backend.Models;

public class Sala{
    [Key]
    public int ID {get; set;}
    public required string Naziv {get; set;}
    public int UkupanBrojRedova {get; set;}
    public int UkupanBrojSedista {get; set;} // mora biti deljivo
    public List<Karta>? Karte {get; set;}
}