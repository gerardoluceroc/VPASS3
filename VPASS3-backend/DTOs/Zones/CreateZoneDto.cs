using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.Zones
{
    public class CreateZoneDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int EstablishmentId { get; set; }
    }
}
