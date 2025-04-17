using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.Directions
{
    public class DirectionDto
    {
        [Required]
        public string? VisitDirection { get; set; }
    }
}
