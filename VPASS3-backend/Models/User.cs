using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models
{
    public class User : IdentityUser
    {
        public int? EstablishmentId { get; set; }

        [JsonIgnore]
        public Establishment establishment { get; set; }
    }
}
