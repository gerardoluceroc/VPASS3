using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.Establishments
{
    public class CreateEstablishmentDto
    {
        [Required]
        public string Name {  get; set; }

        [Required]
        public string Email { get; set; }
    }
}
