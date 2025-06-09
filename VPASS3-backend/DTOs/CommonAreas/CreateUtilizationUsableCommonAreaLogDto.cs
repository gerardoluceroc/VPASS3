using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.CommonAreas
{
    public class CreateUtilizationUsableCommonAreaLogDto
    {
        [Required]
        public int IdUsableCommonArea { get; set; }

        [Required]
        public int IdPerson { get; set; }

        public TimeSpan? UsageTime { get; set; }

        public int? GuestsNumber { get; set; }
    }
}
