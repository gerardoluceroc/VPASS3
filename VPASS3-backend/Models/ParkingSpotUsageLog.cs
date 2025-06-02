using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models
{
    public class ParkingSpotUsageLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdParkingSpot { get; set; }

        [Required]
        public int IdVisitor { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? IdEntryVisit { get; set; }

        public int? IdExitVisit { get; set; }

        // Tiempo autorizado para usar el estacionamiento
        public TimeSpan? AuthorizedTime { get; set; }

        public TimeSpan? UsageTime { get; set; }

        [JsonIgnore]
        public ParkingSpot ParkingSpot { get; set; }

        [JsonIgnore]
        public Visitor Visitor { get; set; }

        [JsonIgnore]
        [ForeignKey("IdEntryVisit")]
        public Visit? EntryVisit { get; set; }

        [JsonIgnore]
        [ForeignKey("IdExitVisit")]
        public Visit? ExitVisit { get; set; }
    }
}