using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.Models.CommonAreas.ReservableCommonArea
{
    public class ReservableCommonArea : CommonArea
    {
        public TimeSpan? MaxReservationTime { get; set; }

        public ICollection<CommonAreaReservation> Reservations { get; set; } = new List<CommonAreaReservation>();
    }
}
