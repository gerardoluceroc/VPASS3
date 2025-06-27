using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using VPASS3_backend.Utils;

namespace VPASS3_backend.Models
{
    public class Package
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdApartment { get; set; }

        [Required]
        public int IdApartmentOwnership { get; set; }

        public string? Code { get; set; }

        [Required]
        public DateTime ReceivedAt { get; set; } = TimeHelper.GetSantiagoTime();

        public DateTime? DeliveredAt { get; set; }

        public int? IdPersonWhoReceived { get; set; }

        [ForeignKey("IdApartment")]
        [JsonIgnore]
        public Apartment Apartment { get; set; }

        [ForeignKey("IdApartmentOwnership")]
        public ApartmentOwnership Ownership { get; set; }

        [ForeignKey("IdPersonWhoReceived")]
        public Person Receiver { get; set; }
    }
}