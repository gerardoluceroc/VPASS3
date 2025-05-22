using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.Visits
{
    public class GetVisitByDatesDto
    {
        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }
    }
}
