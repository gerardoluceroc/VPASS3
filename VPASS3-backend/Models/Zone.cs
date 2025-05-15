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

        // Definición de la propiedad de navegación
        [JsonIgnore]  // Ignorar la propiedad en la serialización JSON para evitar ciclos infinitos
        public Establishment Establishment { get; set; }

        // Relación de uno es a muchos. Una zona tiene muchas subzonas o sub secciones
        public ICollection<ZoneSection> ZoneSections { get; set; } = new List<ZoneSection>();

        // Relación de uno es a muchos. Una zona recibe muchas visitas.
        //public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    }
}