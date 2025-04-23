using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models
{
    public class ParkingSpot
    {
        [Key]
        public int Id { get; set; }

        // Número o nombre identificador del estacionamiento (opcional)
        public string Name { get; set; }

        // Relación con el establecimiento
        [Required]
        public int IdEstablishment { get; set; }

        [JsonIgnore]
        public Establishment Establishment { get; set; }

        // Puedes agregar otras propiedades relevantes como estado o tipo
        //public bool IsAvailable { get; set; } = true;

        [JsonIgnore]
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();

    }
}
