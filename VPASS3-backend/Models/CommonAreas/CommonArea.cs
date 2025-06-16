using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VPASS3_backend.Enums;

namespace VPASS3_backend.Models.CommonAreas
{
    public class CommonArea
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int IdEstablishment { get; set; }

        // Reemplazamos Type por Mode que permite múltiples valores combinados
        [Required]
        public CommonAreaMode Mode { get; set; }

        // Solo se aplica si el área es "usable"
        public int? MaxCapacity { get; set; }

        // Estado actual del área (disponible, en mantenimiento, etc.)
        public CommonAreaStatus Status { get; set; } = CommonAreaStatus.Available;

        // Relación con usos individuales del área (si es usable)
        public ICollection<CommonAreaUsageLog> Usages { get; set; } = new List<CommonAreaUsageLog>();

        // Relación con reservas del área (si es reservable)
        public ICollection<CommonAreaReservation> Reservations { get; set; } = new List<CommonAreaReservation>();

        [JsonIgnore]
        public Establishment Establishment { get; set; }
    }
}
