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

        //Inverso de la relacion muchos es a muchos
        [JsonIgnore]
        public ICollection<CommonAreaReservation> InvitedCommonAreaReservations { get; set; } = new List<CommonAreaReservation>();

        [JsonIgnore]
        public ICollection<Blacklist> Blacklists { get; set; } = new List<Blacklist>();

        [JsonIgnore]
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();

        [JsonIgnore]
        public ICollection<ApartmentOwnership> ApartmentOwnerships { get; set; } = [];
    }
}
