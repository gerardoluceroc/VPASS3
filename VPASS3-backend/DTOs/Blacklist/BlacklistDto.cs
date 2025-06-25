using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.Blacklist
{
    public class BlacklistDto
    {
        //[Required]
        //public int IdVisitor { get; set; }

        [Required]
        public int IdPerson { get; set; }

        [Required]
        public int IdEstablishment { get; set; }

        public string? Reason { get; set; }
    }
}