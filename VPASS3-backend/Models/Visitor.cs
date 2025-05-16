using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models
{
    public class Visitor
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Names { get; set; }

        [Required]
        public string LastNames { get; set; }

        [Required]
        public string IdentificationNumber { get; set; }

        // Relación de uno es a muchos. Un visitante tiene muchas visitas.
        [JsonIgnore]
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();

        [JsonIgnore]
        public ICollection<Blacklist> Blacklists { get; set; } = new List<Blacklist>();
    }
}