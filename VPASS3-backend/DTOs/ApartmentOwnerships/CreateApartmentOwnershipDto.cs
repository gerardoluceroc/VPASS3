using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.ApartmentOwnerships
{
    public class CreateApartmentOwnershipDto
    {
        [Required]
        public int IdApartment { get; set; }

        [Required]
        public int IdPerson { get; set; }
    }
}