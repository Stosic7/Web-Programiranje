using System.Text.Json.Serialization;

namespace WebTemplate.Models
{
    public class Let
    {
        [Key]
        public int ID { get; set; }

        public int BrojPutnika { get; set; }

        public DateTime VremePoletanja { get; set; }

        public DateTime VremeSletanja { get; set; }

        public Aerodrom? PolazniAerodrom { get; set; }

        public Aerodrom? DolazniAerodrom { get; set; }

        public Letelica? Letelica { get; set; }
    }
}