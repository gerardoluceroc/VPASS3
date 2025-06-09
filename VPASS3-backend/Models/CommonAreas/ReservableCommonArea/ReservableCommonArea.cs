using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.Models.CommonAreas.ReservableCommonArea
{
    public class ReservableCommonArea : CommonArea
    {
        public ICollection<CommonAreaReservation> Reservations { get; set; } = new List<CommonAreaReservation>();
    }
}
