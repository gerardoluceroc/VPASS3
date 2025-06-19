using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.CommonAreas
{
    public class CreateUsageLogDto
    {
        [Required] public int IdCommonArea { get; set; }
        [Required] public int IdPerson { get; set; }
        public DateTime? StartTime { get; set; }
        public TimeSpan? UsageTime { get; set; }
        public int? GuestsNumber { get; set; } = 0;

        //public List<int>? GuestIds { get; set; }
    }
}