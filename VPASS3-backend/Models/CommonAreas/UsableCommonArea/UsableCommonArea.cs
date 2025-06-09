using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.Models.CommonAreas.UsableCommonArea
{
    public class UsableCommonArea : CommonArea
    {
        [Required]
        public int MaxCapacity { get; set; }

        // Cada uso individual, con fecha y duración
        public ICollection<UtilizationUsableCommonAreaLog> UtilizationUsableCommonAreaLogs { get; set; } = new List<UtilizationUsableCommonAreaLog>();
    }
}