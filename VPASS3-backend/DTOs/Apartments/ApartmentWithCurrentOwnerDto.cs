using VPASS3_backend.Models;

namespace VPASS3_backend.DTOs.Apartments
{
    public class ApartmentWithCurrentOwnerDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int IdZone { get; set; }
        public bool IsDeleted { get; set; }

        // Zona (Se puede expandir más si se necesitan detalles)
        public string ZoneName { get; set; }

        // Propietario activo (si hay)
        public ApartmentOwnership? ActiveOwnership { get; set; }
    }
}
