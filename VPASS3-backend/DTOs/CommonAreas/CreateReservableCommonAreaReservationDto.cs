using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.CommonAreas
{
    public class CreateReservableCommonAreaReservationDto
    {
        [Required]
        public int IdReservableCommonArea { get; set; }

        [Required]
        public int IdPersonReservedBy { get; set; }

        [Required]
        public TimeSpan ReservationTime { get; set; }
    }
}