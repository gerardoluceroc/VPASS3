using VPASS3_backend.Models;
using VPASS3_backend.Models.CommonAreas;

namespace VPASS3_backend.Interfaces
{
    public interface IUserContextService
    {
        int? UserId { get; }
        string? UserRole { get; }
        string? UserEmail { get; }
        bool IsAuthenticated { get; }

        int? EstablishmentId { get; }

        bool CanAccessOwnResourceById(int resourceOwnerId);

        bool CanAccessOwnResourceByEmail(string resourceOwnerEmail);

        bool CanAccessOwnEstablishment(int establishmentId);

        bool CanAccessOwnEstablishmentByUserId(int resourceOwnerUserId);

        bool CanAccessZone(Zone zone);

        bool CanAccessZoneSection(ZoneSection section);

        bool CanAccessParkingSpot(ParkingSpot spot);

        public bool CanAccessVisit(Visit visit);

        bool CanAccessVisitType(VisitType visitType);

        bool CanAccessVisitor(Visitor visitor);

        bool CanAccessArea(CommonArea area);
    }
}
