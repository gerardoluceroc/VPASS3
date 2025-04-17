using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models
{
    public class Direction
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string VisitDirection { get; set; }

        [JsonIgnore]
        public ICollection<Visit> Visits { get; set; }
    }

}
