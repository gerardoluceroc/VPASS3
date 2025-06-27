using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Context;
using VPASS3_backend.Models;

namespace VPASS3_backend.Utils
{
    public class ApartmentUtils
    {
        /// <summary>
        /// Obtiene el último propietario activo de un departamento (si existe).
        /// </summary>
        /// <param name="dbContext">Instancia del contexto de base de datos.</param>
        /// <param name="apartmentId">ID del departamento.</param>
        /// <returns>ApartmentOwnership si hay propietario activo, de lo contrario null.</returns>

        public static async Task<ApartmentOwnership?> GetActiveOwnershipAsync(AppDbContext dbContext, int apartmentId)
        {
            return await dbContext.ApartmentOwnerships
                .Include(o => o.Person)
                .Where(o => o.IdApartment == apartmentId && o.EndDate == null)
                .OrderByDescending(o => o.Id)
                .FirstOrDefaultAsync();
        }
    }
}
