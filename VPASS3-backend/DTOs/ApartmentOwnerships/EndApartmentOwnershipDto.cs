using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.ApartmentOwnerships
{
    public class EndApartmentOwnershipDto
    {
        [Required]
        public int IdApartment { get; set; }
    }
}