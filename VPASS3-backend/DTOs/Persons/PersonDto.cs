using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.Persons
{
    public class PersonDto
    {
        [Required]
        public string Names { get; set; }

        [Required]
        public string LastNames { get; set; }

        [Required]
        public string IdentificationNumber { get; set; }
    }
}
