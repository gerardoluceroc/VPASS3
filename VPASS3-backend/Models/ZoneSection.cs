using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models
{
    public class ZoneSection
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string? Name { get; set; }  // Puede ser null si la zona no tiene sección

        [Required]
        public int IdZone { get; set; }

        public bool IsDeleted { get; set; } = false;

        [JsonIgnore]
        public Zone Zone { get; set; }

        //[JsonIgnore]
        //public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    }
}
