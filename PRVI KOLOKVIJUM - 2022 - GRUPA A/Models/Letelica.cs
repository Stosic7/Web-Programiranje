using System.Text.Json.Serialization;

namespace WebTemplate.Models
{
    public class Letelica
    {
        [Key]
        public int ID { get; set; }

        public required string Naziv { get; set; }

        public int KapacitetPutnika { get; set; }

        public int BrojOsoblja { get; set; }

        public int BrojMotora { get; set; }
    }
}