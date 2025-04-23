using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VPASS3_backend.Models;

namespace VPASS3_backend.DTOs.ZoneSections
{
    public class ZoneSectionDto
    {
        [Required]
        public string? Name { get; set; }  // Puede ser null si la zona no tiene sección

        [Required]
        public int IdZone { get; set; }

    }
}
