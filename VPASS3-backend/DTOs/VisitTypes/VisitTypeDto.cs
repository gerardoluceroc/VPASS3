using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.VisitTypes
{
    public class VisitTypeDto
    {
        [Required]
        public string Name { get; set; }

    }
}
