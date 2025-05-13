using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.DTOs.ParkingSpots
{
    public class UpdateParkingSpotDto
    {
        public string Name { get; set; }

        public bool? IsAvailable { get; set; }

        [Required]
        public int IdEstablishment { get; set; }
    }
}
