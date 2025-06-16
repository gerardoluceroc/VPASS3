using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.CommonAreas
{
    public class CreateUsageLogDto
    {
        [Required] public int IdCommonArea { get; set; }
        [Required] public int IdPerson { get; set; }
        [Required] public DateTime StartTime { get; set; }
        [Required] public TimeSpan UsageTime { get; set; }
        public List<int>? GuestIds { get; set; }
    }
}