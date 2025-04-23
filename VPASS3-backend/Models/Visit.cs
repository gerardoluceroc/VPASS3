using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models
{
    public class Visit
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int EstablishmentId { get; set; }

        [Required]
        public int VisitorId { get; set; }

        [Required]
        public int ZoneId { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }

        [Required]
        public int IdDirection { get; set; }

        public int? IdZoneSection { get; set; }

        [Required]
        public bool VehicleIncluded { get; set; }

        // Nueva propiedad opcional: patente del vehículo
        public string? LicensePlate { get; set; }

        // Nueva propiedad opcional: FK al estacionamiento usado
        public int? IdParkingSpot { get; set; }

        [Required]
        public int IdVisitType { get; set; }

        [JsonIgnore]
        public ParkingSpot? ParkingSpot { get; set; }

        // Propiedades de navegación

        [JsonIgnore]
        public VisitType VisitType { get; set; }


        [JsonIgnore]
        public Direction Direction { get; set; }

        [JsonIgnore]
        public Establishment Establishment { get; set; }

        [JsonIgnore]
        public Visitor Visitor { get; set; }

        [JsonIgnore]
        public Zone Zone { get; set; }

        [JsonIgnore]
        [ForeignKey("IdZoneSection")]
        public ZoneSection? ZoneSection { get; set; }
    }
}
