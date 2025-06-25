using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.Apartments
{
    public class ApartmentDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int IdZone { get; set; }
    }
}
