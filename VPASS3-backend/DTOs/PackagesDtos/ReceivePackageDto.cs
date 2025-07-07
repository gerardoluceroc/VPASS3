using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.PackagesDtos
{
    public class ReceivePackageDto
    {
        [Required] public int IdPackage { get; set; }
        public int? IdPersonWhoReceived { get; set; }
    }
}
