using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs
{
    public class RoleDto
    {
        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        public string Name { get; set; } = string.Empty;
    }


}
