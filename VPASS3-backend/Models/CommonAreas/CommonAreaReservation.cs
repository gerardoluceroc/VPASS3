using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models.CommonAreas
{
    public class CommonAreaReservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime ReservationStart { get; set; }

        public TimeSpan? ReservationTime { get; set; }

        public DateTime? ReservationEnd => ReservationTime.HasValue
        ? ReservationStart + ReservationTime.Value
        : (DateTime?)null;

        [Required]
        public int IdPersonReservedBy { get; set; }

        [ForeignKey("IdPersonReservedBy")]
        public Person ReservedBy { get; set; }

        // Invitados opcionales
        //public ICollection<Person> Guests { get; set; } = new List<Person>();

        //// Número de invitados calculado
        //[NotMapped]
        //public int GuestsCount => Guests?.Count ?? 0;

        public int? GuestsNumber { get; set; } = 0;

        [Required]
        public int IdCommonArea { get; set; }

        [ForeignKey("IdCommonArea")]
        [JsonIgnore]
        public CommonArea CommonArea { get; set; }
    }
}