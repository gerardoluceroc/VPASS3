using System.ComponentModel.DataAnnotations;
using VPASS3_backend.Enums;

namespace VPASS3_backend.DTOs.CommonAreas
{
    public class CreateCommonAreaDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int IdEstablishment { get; set; }

        [Required]
        public CommonAreaMode Mode { get; set; }

        public int? MaxCapacity { get; set; }
    }
}