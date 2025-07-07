using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.PackagesDtos
{
    public class CreatePackageDto
    {
        [Required] 
        public int IdApartment { get; set; }

        [Required] 
        public int IdApartmentOwnership { get; set; }

        [Required]
        public string Recipient { get; set; }

        public string? Code { get; set; }

        public int? IdPersonWhoReceived { get; set; }

    }
}