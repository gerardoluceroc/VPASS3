using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.Models
{
    public class Establishment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Relación uno a muchos: un establecimiento tiene muchos usuarios
        public ICollection<User> Users { get; set; }
    }
}
