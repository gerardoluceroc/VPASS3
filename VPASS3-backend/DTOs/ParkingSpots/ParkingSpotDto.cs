using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.ParkingSpots
{
    public class ParkingSpotDto
    {
        public string Name { get; set; }

        [Required]
        public int IdEstablishment { get; set; }
    }
}
