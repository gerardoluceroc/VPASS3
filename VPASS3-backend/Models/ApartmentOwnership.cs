using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using VPASS3_backend.Utils;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models
{
    public class ApartmentOwnership
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdApartment { get; set; }

        [Required]
        [JsonIgnore]
        public int IdPerson { get; set; }

        public DateTime StartDate { get; set; } = TimeHelper.GetSantiagoTime();
        public DateTime? EndDate { get; set; }

        [ForeignKey("IdApartment")]
        [JsonIgnore]
        public Apartment Apartment { get; set; }

        [ForeignKey("IdPerson")]
        public Person Person { get; set; }
    }
}