using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.CommonAreas
{
    public class UpdateUsageLogDto
    {
        [Required] public DateTime StartTime { get; set; }
        [Required] public TimeSpan UsageTime { get; set; }
        public List<int>? GuestIds { get; set; }
    }
}