using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.Models.CommonAreas.ReservableCommonArea
{
    public class CommonAreaReservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdReservableCommonArea { get; set; }

        [Required]
        public DateTime ReservationStart { get; set; }

        [Required]
        public DateTime ReservationEnd { get; set; }

        [Required]
        public int IdVisitorReservedBy { get; set; }

        public Visitor ReservedBy { get; set; }

        public ICollection<ReservationCommonAreaGuest> Guests { get; set; } = new List<ReservationCommonAreaGuest>();

        [ForeignKey("IdReservableCommonArea")]
        public ReservableCommonArea CommonArea { get; set; }
    }

}
