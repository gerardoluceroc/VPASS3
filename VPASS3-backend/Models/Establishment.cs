using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models
{
    public class Establishment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int IdUser { get; set; }

        // Relación uno a uno: un establecimiento tiene un solo usuario
        [JsonIgnore]
        public User User { get; set; }

        // Relación de uno a muchos: Un Establishment tiene muchas Zonas
        public ICollection<Zone> Zones { get; set; } = new List<Zone>();

        // Relación de uno es a muchos. Un establecimiento tiene muchas visitas.
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();

        // Relación de uno es a muchos. Un establecimiento tiene muchos espacios de estacionamiento.
        public ICollection<ParkingSpot> ParkingSpots { get; set; } = new List<ParkingSpot>();

    }
}
