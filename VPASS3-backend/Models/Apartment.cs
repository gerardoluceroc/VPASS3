using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models
{
    public class Apartment
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string? Name { get; set; }

        [Required]
        public int IdZone { get; set; }

        public bool IsDeleted { get; set; } = false;

        [JsonIgnore]
        public Zone Zone { get; set; }
    }
}