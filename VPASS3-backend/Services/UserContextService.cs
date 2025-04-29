using System.Security.Claims;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;

namespace VPASS3_backend.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Usamos TryParse para convertir el valor de NameIdentifier a int? de forma segura
        public int? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Intentamos convertir el valor de userIdClaim a un entero
                if (int.TryParse(userIdClaim, out int userId))
                {
                    return userId;
                }

                // Si no se puede convertir, retornamos null
                return null;
            }
        }

        public int? EstablishmentId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst("establishment_id")?.Value;
                return int.TryParse(claim, out var estId) ? estId : null;
            }
        }

        public string? UserRole =>
            _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

        public string? UserEmail =>
            _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

        public bool CanAccessOwnResourceById(int resourceOwnerId)
        {
            // SUPERADMIN puede acceder a todo
            if (UserRole == "SUPERADMIN")
                return true;

            // Otros usuarios solo acceden a su propio recurso
            return UserId == resourceOwnerId;
        }

        public bool CanAccessOwnResourceByEmail(string resourceOwnerEmail)
        {
            // SUPERADMIN puede acceder a todo
            if (UserRole == "SUPERADMIN")
                return true;

            // Otros usuarios solo acceden a su propio recurso
            return UserEmail == resourceOwnerEmail;
        }

        public bool CanAccessOwnEstablishment(int establishmentId)
        {
            // SUPERADMIN puede acceder a todos los establecimientos
            if (UserRole == "SUPERADMIN")
                return true;

            // Comparar el ID del establecimiento del token con el recibido
            return EstablishmentId.HasValue && EstablishmentId.Value == establishmentId;
        }

        public bool CanAccessOwnEstablishmentByUserId(int resourceOwnerUserId)
        {
            // SUPERADMIN puede acceder a todos los recursos
            if (UserRole == "SUPERADMIN")
                return true;

            // Otros usuarios solo acceden si son dueños del recurso
            return UserId.HasValue && UserId.Value == resourceOwnerUserId;
        }

        public bool CanAccessZone(Zone zone)
        {
            if (zone == null)
                return false;

            // SUPERADMIN puede acceder a todo
            if (UserRole == "SUPERADMIN")
                return true;

            // ADMIN solo puede acceder a zonas de su establecimiento
            return EstablishmentId.HasValue && EstablishmentId.Value == zone.EstablishmentId;
        }

        public bool CanAccessZoneSection(ZoneSection section)
        {
            if (section == null)
                return false;

            if (UserRole == "SUPERADMIN")
                return true;

            return EstablishmentId.HasValue
                && section.Zone != null
                && section.Zone.EstablishmentId == EstablishmentId.Value;
        }

        public bool CanAccessParkingSpot(ParkingSpot spot)
        {
            if (spot == null)
                return false;

            if (UserRole == "SUPERADMIN")
                return true;

            return EstablishmentId.HasValue && EstablishmentId.Value == spot.IdEstablishment;
        }

        public bool CanAccessVisit(Visit visit)
        {
            if (visit == null)
                return false;

            if (UserRole == "SUPERADMIN")
                return true;

            return EstablishmentId.HasValue && EstablishmentId.Value == visit.EstablishmentId;
        }

        public bool CanAccessVisitType(VisitType visitType)
        {
            if (visitType == null)
                return false;

            if (UserRole == "SUPERADMIN")
                return true;

            return EstablishmentId.HasValue && EstablishmentId.Value == visitType.IdEstablishment;
        }

        public bool CanAccessVisitor(Visitor visitor)
        {
            if (visitor == null)
                return false;

            if (UserRole == "SUPERADMIN")
                return true;

            // Validar si al menos una visita del visitante pertenece al establecimiento del usuario
            return EstablishmentId.HasValue &&
                   visitor.Visits.Any(v => v.EstablishmentId == EstablishmentId.Value);
        }




    }
}

