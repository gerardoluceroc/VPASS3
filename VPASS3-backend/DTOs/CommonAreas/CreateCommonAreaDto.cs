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
        [EnumDataType(typeof(CommonAreaType))]
        public CommonAreaType Type { get; set; }

        // Solo requerido si el área es de tipo Usable
        public int? MaxCapacity { get; set; }
    }
}
