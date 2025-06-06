using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models.CommonAreas.ReservableCommonArea
{
    public class ReservationCommonAreaGuest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdVisitor { get; set; }

        public int IdReservation { get; set; }

        [ForeignKey("IdVisitor")]
        public Visitor Visitor { get; set; }

        [JsonIgnore]
        [ForeignKey("IdReservation")]
        public CommonAreaReservation Reservation { get; set; }
    }
}
