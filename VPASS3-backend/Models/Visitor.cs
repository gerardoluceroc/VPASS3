using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.Models
{
    public class Visitor
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Names { get; set; }

        [Required]
        public string LastNames { get; set; }

        [Required]
        public string IdentificationNumber { get; set; }

        // Relación de uno es a muchos. Un visitante tiene muchas visitas.
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    }
}
