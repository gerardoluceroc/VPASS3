using System.ComponentModel.DataAnnotations;
using VPASS3_backend.Enums;

namespace VPASS3_backend.DTOs.CommonAreas
{
    public class UpdateCommonAreaDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public CommonAreaMode Mode { get; set; }

        public int? MaxCapacity { get; set; }

        public CommonAreaStatus? Status { get; set; }
    }
}
