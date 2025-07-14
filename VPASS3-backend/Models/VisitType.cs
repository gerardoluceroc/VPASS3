using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models
{
    public class VisitType
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int IdEstablishment { get; set; }

        [JsonIgnore]
        [ForeignKey("IdEstablishment")]
        public Establishment Establishment { get; set; }
    }
}