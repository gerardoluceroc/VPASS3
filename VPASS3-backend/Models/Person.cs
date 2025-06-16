using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VPASS3_backend.Models.CommonAreas;

namespace VPASS3_backend.Models
{
    public class Person
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Names { get; set; }

        [Required]
        public string LastNames { get; set; }

        [Required]
        public string IdentificationNumber { get; set; }

        //Inverso de la rerlacion muchos es a muchos
        [JsonIgnore]
        public ICollection<CommonAreaReservation> InvitedCommonAreaReservations { get; set; } = new List<CommonAreaReservation>();
    }
}
