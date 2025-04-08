using Microsoft.AspNetCore.Identity;

namespace VPASS3_backend.Models
{
    public class User : IdentityUser
    {
        // Relación muchos a muchos con los roles
        //public ICollection<IdentityUserRole<string>> UserRoles { get; set; }
    }
}
