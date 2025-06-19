using VPASS3_backend.Context;
using VPASS3_backend.DTOs.CommonAreas;
using VPASS3_backend.DTOs;
using VPASS3_backend.Enums;
using VPASS3_backend.Interfaces.CommonAreaInterfaces;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models.CommonAreas;
using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Utils;

namespace VPASS3_backend.Services.CommonAreaServices
{
    public class CommonAreaUsageLogService : ICommonAreaUsageLogService
    {
        private readonly AppDbContext _ctx;
        private readonly IUserContextService _userCtx;
        private readonly IAuditLogService _audit;

        public CommonAreaUsageLogService(AppDbContext ctx, IUserContextService userCtx, IAuditLogService audit)
        {
            _ctx = ctx;
            _userCtx = userCtx;
            _audit = audit;
        }

        public async Task<ResponseDto> GetAllAsync()
        {
            try
            {
                // Obtenemos los usos con personas e invitados
                IQueryable<CommonAreaUsageLog> q = _ctx.CommonAreaUsageLogs
                    .Include(u => u.Person);

                // Si el usuario NO es superadmin, filtramos por establecimiento usando un join
                if (_userCtx.UserRole != "SUPERADMIN")
                {
                    var estId = _userCtx.EstablishmentId ?? 0;

                    q = q.Where(u =>
                        _ctx.CommonAreas
                            .Any(ca => ca.Id == u.IdCommonArea && ca.IdEstablishment == estId)
                    );
                }

                var list = await q.ToListAsync();
                return new ResponseDto(200, list, "Usos obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllAsync (UsageLog): " + ex.Message);
                return new ResponseDto(500, message: "Error al obtener los registros de uso.");
            }
        }


        public async Task<ResponseDto> GetByIdAsync(int id)
        {
            var u = await _ctx.CommonAreaUsageLogs
                .Include(x => x.Person)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (u == null) return new ResponseDto(404, message: "Uso no encontrado.");

            if (_userCtx.UserRole != "SUPERADMIN" && u.CommonArea.IdEstablishment != _userCtx.EstablishmentId)
                return new ResponseDto(403, message: "Sin permiso para ver este uso.");

            return new ResponseDto(200, u, "Uso obtenido correctamente.");
        }

        public async Task<ResponseDto> CreateUsageAsync(CreateUsageLogDto dto)
        {
            var area = await _ctx.CommonAreas.FindAsync(dto.IdCommonArea);

            if (area == null) return new ResponseDto(404, message: "Área común no existe.");
            if (!area.Mode.HasFlag(CommonAreaMode.Usable))
                return new ResponseDto(400, message: "El área no permite uso.");
            if (!_userCtx.CanAccessArea(area))
                return new ResponseDto(403, message: "Sin permiso para usar este espacio.");
            if (area.Status != CommonAreaStatus.Available)
                return new ResponseDto(400, message: "Área no disponible actualmente.");

            var person = await _ctx.Persons.FindAsync(dto.IdPerson);
            if (person == null)
                return new ResponseDto(404, message: "Persona no encontrada.");

            var startTime = dto.StartTime.HasValue && dto.StartTime.Value != default
                ? dto.StartTime.Value
                : TimeHelper.GetSantiagoTime();

            var usage = new CommonAreaUsageLog
            {
                IdCommonArea = dto.IdCommonArea,
                IdPerson = dto.IdPerson,
                StartTime = startTime,
                UsageTime = dto.UsageTime,
                GuestsNumber = dto.GuestsNumber ?? 0
            };

            _ctx.CommonAreaUsageLogs.Add(usage);
            await _ctx.SaveChangesAsync();

            //// Si decides usar invitados en el futuro:
            //if (dto.GuestIds?.Any() == true)
            //{
            //    var guests = await _ctx.Persons.Where(p => dto.GuestIds.Contains(p.Id)).ToListAsync();
            //    usage.InvitedGuests = guests;
            //    await _ctx.SaveChangesAsync();
            //}

            await _audit.LogManualAsync(
                action: $"Nuevo uso registrado en área común '{area.Name}'",
                email: _userCtx.UserEmail,
                role: _userCtx.UserRole,
                userId: _userCtx.UserId ?? 0,
                endpoint: "/Usage/create",
                httpMethod: "POST",
                statusCode: 201
            );

            return new ResponseDto(201, usage, "Uso registrado correctamente.");
        }

        public async Task<ResponseDto> UpdateAsync(int id, UpdateUsageLogDto dto)
        {
            var usage = await _ctx.CommonAreaUsageLogs
                .Include(u => u.CommonArea)
                //.Include(u => u.InvitedGuests)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usage == null)
                return new ResponseDto(404, message: "Registro de uso no encontrado.");

            if (!_userCtx.CanAccessArea(usage.CommonArea))
                return new ResponseDto(403, message: "No tienes permisos para editar este uso.");

            if (!usage.CommonArea.Mode.HasFlag(CommonAreaMode.Usable))
                return new ResponseDto(400, message: "El área no está habilitada para usos.");

            if (usage.CommonArea.Status != CommonAreaStatus.Available)
                return new ResponseDto(400, message: "El área no está disponible.");

            usage.StartTime = dto.StartTime.HasValue && dto.StartTime.Value != default
                ? dto.StartTime.Value
                : TimeHelper.GetSantiagoTime();

            usage.UsageTime = dto.UsageTime;
            usage.GuestsNumber = dto.GuestsNumber ?? 0;

            //// Invitados no se manejan en esta etapa
            //usage.InvitedGuests.Clear();
            //if (dto.GuestIds?.Any() == true)
            //{
            //    var guests = await _ctx.Persons
            //        .Where(p => dto.GuestIds.Contains(p.Id))
            //        .ToListAsync();

            //    usage.InvitedGuests = guests;
            //}

            await _ctx.SaveChangesAsync();
            return new ResponseDto(200, usage, "Uso actualizado correctamente.");
        }

        public async Task<ResponseDto> DeleteUsageAsync(int id)
        {
            var u = await _ctx.CommonAreaUsageLogs
                .Include(x => x.CommonArea)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (u == null) return new ResponseDto(404, message: "Uso no encontrado.");

            if (_userCtx.UserRole != "SUPERADMIN" && u.CommonArea.IdEstablishment != _userCtx.EstablishmentId)
                return new ResponseDto(403, message: "Sin permiso para eliminar este uso.");

            _ctx.CommonAreaUsageLogs.Remove(u);
            await _ctx.SaveChangesAsync();

            return new ResponseDto(200, message: "Uso eliminado correctamente.");
        }
    }
}