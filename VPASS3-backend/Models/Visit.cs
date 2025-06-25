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
        [JsonIgnore]
        public int IdPerson { get; set; }

        [Required]
        [JsonIgnore]
        public int ZoneId { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }

        [Required]
        [JsonIgnore]
        public int IdDirection { get; set; }

        [JsonIgnore]
        public int? IdApartment { get; set; }

        [Required]
        public bool VehicleIncluded { get; set; }

        // Nueva propiedad opcional: patente del vehículo
        public string? LicensePlate { get; set; }

        // Nueva propiedad opcional: tiempo autorizado para la visita para usar el estacionamiento
        public TimeSpan? AuthorizedTime { get; set; }

        // Nueva propiedad opcional: FK al estacionamiento usado
        [JsonIgnore]
        public int? IdParkingSpot { get; set; }

        [Required]
        [JsonIgnore]
        public int IdVisitType { get; set; }

        //Propiedades de navegacion
        //[JsonIgnore]
        public ParkingSpot? ParkingSpot { get; set; }

        // Propiedades de navegación

        //[JsonIgnore]
        public VisitType VisitType { get; set; }


        //[JsonIgnore]
        public Direction Direction { get; set; }

        [JsonIgnore]
        public Establishment Establishment { get; set; }

        //[JsonIgnore]
        [ForeignKey("IdPerson")]
        public Person Person { get; set; }

        //[JsonIgnore]
        public Zone Zone { get; set; }

        [ForeignKey("IdApartment")]
        //[JsonIgnore]
        public Apartment? Apartment { get; set; }
    }
}