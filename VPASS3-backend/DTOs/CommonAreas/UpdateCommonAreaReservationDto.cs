using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.CommonAreas
{
    public class UpdateCommonAreaReservationDto
    {
        [Required] public DateTime ReservationStart { get; set; }
        [Required] public TimeSpan ReservationTime { get; set; }
        public List<int>? GuestIds { get; set; } = new();
    }
}
