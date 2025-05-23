using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.Visits
{
    public class ExportVisitsExcelByIdentificationNumberDto
    {
        [Required]
        public string IdentificationNumber { get; set; }
    }
}
