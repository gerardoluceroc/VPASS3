using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models
{
    public class ParkingSpot
    {
        [Key]
        public int Id { get; set; }

        // Número o nombre identificador del estacionamiento (opcional)
        public string Name { get; set; }

        public bool? IsAvailable { get; set; }

        // Relación con el establecimiento
        [Required]
        public int IdEstablishment { get; set; }

        [JsonIgnore]
        public Establishment Establishment { get; set; }

        [JsonIgnore]
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();

        [JsonIgnore]
        public ICollection<ParkingSpotUsageLog> UsageLogs { get; set; } = new List<ParkingSpotUsageLog>();


    }
}
