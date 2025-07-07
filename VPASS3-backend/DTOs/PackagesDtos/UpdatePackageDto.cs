using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.PackagesDtos
{
    public class UpdatePackageDto
    {
        [Required] public int Id { get; set; }
        public string? Recipient { get; set; }
        public string? Code { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public int? IdPersonWhoReceived { get; set; }
    }
}
