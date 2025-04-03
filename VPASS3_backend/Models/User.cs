using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public int RoleId { get; set; } // Relación con Role
        public Role Role { get; set; } // Relación con el rol (solo para navegación, no es necesario si no lo usas)
    }

}



