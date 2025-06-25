using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models
{
    public class Zone
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int EstablishmentId { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Definición de la propiedad de navegación
        [JsonIgnore]  // Ignorar la propiedad en la serialización JSON para evitar ciclos infinitos
        public Establishment Establishment { get; set; }

        // Relación de uno es a muchos. Una zona tiene muchos departamentos
        public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
    }
}