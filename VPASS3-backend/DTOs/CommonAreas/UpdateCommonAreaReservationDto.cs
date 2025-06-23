using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.CommonAreas
{
    public class UpdateCommonAreaReservationDto
    {
        public DateTime? ReservationStart { get; set; }
        public TimeSpan? ReservationTime { get; set; }
        public int? GuestsNumber { get; set; } = 0;

        //public List<int>? GuestIds { get; set; } = new();
    }
}