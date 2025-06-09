using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VPASS3_backend.Enums;

namespace VPASS3_backend.Models.CommonAreas
{
    public abstract class CommonArea
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int IdEstablishment { get; set; }

        [JsonIgnore]
        public Establishment Establishment { get; set; }

        [Required]
        public CommonAreaType Type { get; set; } // Enum: Reservable, Utilizable
    }
}
