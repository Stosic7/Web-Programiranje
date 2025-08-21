using System.Text.Json.Serialization;

namespace backend.Models;

public class Karta{
    [Key]
    public int ID {get; set;}
    public bool Placena {get; set;}
    public bool Zauzeto {get; set;}
    public decimal Cena { get; set; }
    public int Red {get; set;}
    public int Sediste {get; set;}
    public int sifraProjekcije {get; set;}
    public Sala? Sala {get; set;}
    public Projekcija? Projekcija {get; set;}
}
