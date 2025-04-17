using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models
{
    public class Visit
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int EstablishmentId { get; set; }

        [Required]
        public int VisitorId { get; set; }

        [Required]
        public int ZoneId { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }

        [Required]
        public int IdDirection { get; set; }

        // Propiedades de navegación

        [JsonIgnore]
        public Direction Direction { get; set; }

        [JsonIgnore]
        public Establishment Establishment { get; set; }

        [JsonIgnore]
        public Visitor Visitor { get; set; }

        [JsonIgnore]
        public Zone Zone { get; set; }
    }
}
