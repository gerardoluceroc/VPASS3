namespace VPASS3_backend.DTOs.CommonAreas
{
    public class UpdateUsageLogDto
    {
        public DateTime? StartTime { get; set; }
        public TimeSpan? UsageTime { get; set; }
        public int? GuestsNumber { get; set; } = 0;

        //public List<int>? GuestIds { get; set; }
    }
}
