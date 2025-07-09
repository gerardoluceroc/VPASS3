using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.PackagesDtos
{
    public class GetPackagesByDatesDto
    {
        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }
    }
}
