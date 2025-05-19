using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models
{
    public class Blacklist
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [JsonIgnore]
        public int IdVisitor { get; set; }

        [Required]
        public int IdEstablishment { get; set; }

        public string? Reason { get; set; }

        // Propiedades de navegaciónssage: "No tienes permisos para este establecimiento."));

        public Visitor Visitor { get; set; }

        [JsonIgnore]
        public Establishment Establishment { get; set; }

    }
}