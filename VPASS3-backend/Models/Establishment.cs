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

        // Relación uno a muchos: un establecimiento tiene asociado muchos usuarios
        [JsonIgnore]
        public ICollection<User> Users { get; set; } = new List<User>();

        // Relación de uno a muchos: Un Establishment tiene muchas Zonas
        public ICollection<Zone> Zones { get; set; } = new List<Zone>();

        // Relación de uno es a muchos. Un establecimiento tiene muchas visitas.
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();

        // Relación de uno es a muchos. Un establecimiento tiene muchos espacios de estacionamiento.
        public ICollection<ParkingSpot> ParkingSpots { get; set; } = new List<ParkingSpot>();

        [JsonIgnore]
        public ICollection<Blacklist> Blacklists { get; set; } = new List<Blacklist>();
    }
}