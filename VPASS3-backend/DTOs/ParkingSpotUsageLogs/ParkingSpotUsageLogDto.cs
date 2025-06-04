using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.ParkingSpotUsageLogs
{
    public class ParkingSpotUsageLogDto
    {
        [Required]
        public int IdVisit { get; set; }
    }
}