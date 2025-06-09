using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models.CommonAreas.ReservableCommonArea
{
    public class CommonAreaReservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public TimeSpan? ReservationTime { get; set; }

        [Required]
        public DateTime ReservationStart { get; set; }

        public DateTime? ReservationEnd { get; set; }

        public ICollection<Person> Guests { get; set; } = new List<Person>();

        [Required]
        [JsonIgnore]
        public int IdPersonReservedBy { get; set; }

        [ForeignKey("IdPersonReservedBy")]
        public Person ReservedBy { get; set; }

        [Required]
        public int IdReservableCommonArea { get; set; }

        [ForeignKey("IdReservableCommonArea")]
        [JsonIgnore]
        public ReservableCommonArea ReservableCommonArea { get; set; }
    }
}