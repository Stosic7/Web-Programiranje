using System.Text.Json.Serialization;

namespace WebTemplate.Models
{
    public class Aerodrom
    {
        [Key]
        public int ID { get; set; }

        public required string Naziv { get; set; }

        [MaxLength(3)]
        public required string Kod { get; set; }

        public required string Lokacija { get; set; }

        public int KapacitetLetelica { get; set; }

        public int KapacitetPutnika { get; set; }

        [JsonIgnore]
        public List<Let>? PolazniLetovi { get; set; }

        [JsonIgnore]
        public List<Let>? DolazniLetovi { get; set; }
    }
}