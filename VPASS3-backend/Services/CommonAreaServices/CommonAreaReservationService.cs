using VPASS3_backend.Context;
using VPASS3_backend.DTOs.CommonAreas;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces.CommonAreaInterfaces;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models.CommonAreas;
using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Enums;
using VPASS3_backend.Utils;

namespace VPASS3_backend.Services.CommonAreaServices
{
    public class CommonAreaReservationService : ICommonAreaReservationService
    {
        private readonly AppDbContext _ctx;
        private readonly IUserContextService _userCtx;
        private readonly IAuditLogService _audit;

        public CommonAreaReservationService(AppDbContext ctx, IUserContextService userCtx, IAuditLogService audit)
        {
            _ctx = ctx;
            _userCtx = userCtx;
            _audit = audit;
        }

        public async Task<ResponseDto> GetAllAsync()
        {
            try
            {
                IQueryable<CommonAreaReservation> query = _ctx.CommonAreaReservations
                .Include(r => r.ReservedBy);

                if (_userCtx.UserRole != "SUPERADMIN")
                {
                    if (!_userCtx.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "No tienes un establecimiento asociado.");

                    query = query.Where(r => r.CommonArea.IdEstablishment == _userCtx.EstablishmentId.Value);
                }

                var data = await query.ToListAsync();
                return new ResponseDto(200, data, "Reservas obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ResponseDto(500, message: "Error al obtener reservas.");
            }
        }

        public async Task<ResponseDto> GetByIdAsync(int id)
        {
            var r = await _ctx.CommonAreaReservations
                .Include(r => r.ReservedBy)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (r == null) return new ResponseDto(404, message: "Reserva no encontrada.");
            if (_userCtx.UserRole != "SUPERADMIN" &&
                (!_userCtx.EstablishmentId.HasValue ||
                 r.CommonArea.IdEstablishment != _userCtx.EstablishmentId.Value))
                return new ResponseDto(403, message: "No puedes ver esta reserva.");

            return new ResponseDto(200, r, "Reserva obtenida correctamente.");
        }

        public async Task<ResponseDto> CreateAsync(CreateCommonAreaReservationDto dto)
        {
            var area = await _ctx.CommonAreas.FindAsync(dto.IdCommonArea);

            if (area == null)
                return new ResponseDto(404, message: "Área común no existe.");

            if (!_userCtx.CanAccessArea(area))
                return new ResponseDto(403, message: "No puedes reservar este espacio.");

            if (!area.Mode.HasFlag(CommonAreaMode.Reservable))
                return new ResponseDto(400, message: "El área no está habilitada para reservas.");

            var person = await _ctx.Persons.FindAsync(dto.IdPersonReservedBy);
            if (person == null)
                return new ResponseDto(404, message: "Persona que reserva no encontrada.");

            // Asignar hora de inicio, usando hora actual si no se especifica
            var reservationStart = dto.ReservationStart.HasValue && dto.ReservationStart.Value != default
                ? dto.ReservationStart.Value
                : TimeHelper.GetSantiagoTime();

            // Calcular hora de fin
            var reservationEnd = reservationStart + (dto.ReservationTime ?? TimeSpan.Zero);
            var now = TimeHelper.GetSantiagoTime();

            if (reservationEnd <= now)
                return new ResponseDto(400, message: "La reserva no puede estar en el pasado.");

            // Validar conflictos de horario
            var overlapping = await _ctx.CommonAreaReservations
                .Where(r => r.IdCommonArea == dto.IdCommonArea && r.ReservationTime != null)
                .ToListAsync();

            var isOverlapping = overlapping.Any(r =>
            {
                var existingStart = r.ReservationStart;
                var existingEnd = r.ReservationEnd ?? existingStart;
                return reservationStart < existingEnd && reservationEnd > existingStart;
            });

            if (isOverlapping)
                return new ResponseDto(400, message: "Ya existe una reserva en este horario para esta área.");

            // Crear reserva
            var reservation = new CommonAreaReservation
            {
                IdCommonArea = dto.IdCommonArea,
                ReservationStart = reservationStart,
                ReservationTime = dto.ReservationTime,
                IdPersonReservedBy = dto.IdPersonReservedBy,
                GuestsNumber = dto.GuestsNumber ?? 0
            };

            _ctx.CommonAreaReservations.Add(reservation);
            await _ctx.SaveChangesAsync();

            //if (dto.GuestIds?.Any() == true)
            //{
            //    var guests = await _ctx.Persons.Where(p => dto.GuestIds.Contains(p.Id)).ToListAsync();
            //    reservation.Guests = guests;
            //    await _ctx.SaveChangesAsync();
            //}

            return new ResponseDto(201, reservation, "Reserva creada.");
        }

        public async Task<ResponseDto> UpdateAsync(int id, UpdateCommonAreaReservationDto dto)
        {
            var r = await _ctx.CommonAreaReservations
                .Include(r => r.CommonArea)
                //.Include(r => r.Guests)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (r == null)
                return new ResponseDto(404, message: "Reserva no encontrada.");

            if (!_userCtx.CanAccessArea(r.CommonArea))
                return new ResponseDto(403, message: "No puedes editar esta reserva.");

            if (!r.CommonArea.Mode.HasFlag(CommonAreaMode.Reservable))
                return new ResponseDto(400, message: "El área no está habilitada para reservas.");

            // Determinar los nuevos valores (o mantener los actuales)
            var reservationStart = dto.ReservationStart ?? r.ReservationStart;
            var reservationTime = dto.ReservationTime ?? r.ReservationTime;
            var reservationEnd = reservationStart + (reservationTime ?? TimeSpan.Zero);

            var now = TimeHelper.GetSantiagoTime();
            if (reservationEnd <= now)
                return new ResponseDto(400, message: "La reserva no puede estar en el pasado.");

            // Verificar traslape con otras reservas (excluyendo la actual)
            var overlapping = await _ctx.CommonAreaReservations
                .Where(x =>
                    x.IdCommonArea == r.IdCommonArea &&
                    x.Id != r.Id &&
                    x.ReservationTime != null)
                .ToListAsync();

            var isOverlapping = overlapping.Any(o =>
            {
                var existingStart = o.ReservationStart;
                var existingEnd = o.ReservationEnd ?? existingStart;
                return reservationStart < existingEnd && reservationEnd > existingStart;
            });

            if (isOverlapping)
                return new ResponseDto(400, message: "Ya existe una reserva en este horario para esta área.");

            // Asignar nuevos valores
            r.ReservationStart = reservationStart;
            r.ReservationTime = reservationTime;
            r.GuestsNumber = dto.GuestsNumber ?? r.GuestsNumber;

            //// Actualización futura de invitados
            //r.Guests.Clear();
            //if (dto.GuestIds?.Any() == true)
            //{
            //    var guests = await _ctx.Persons
            //        .Where(p => dto.GuestIds.Contains(p.Id))
            //        .ToListAsync();
            //    r.Guests = guests;
            //}

            await _ctx.SaveChangesAsync();
            return new ResponseDto(200, r, "Reserva actualizada.");
        }




        public async Task<ResponseDto> DeleteAsync(int id)
        {
            var r = await _ctx.CommonAreaReservations
                .Include(r => r.CommonArea)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (r == null) return new ResponseDto(404, message: "Reserva no encontrada.");
            if (!_userCtx.CanAccessArea(r.CommonArea))
                return new ResponseDto(403, message: "No puedes eliminar esta reserva.");

            _ctx.CommonAreaReservations.Remove(r);
            await _ctx.SaveChangesAsync();
            return new ResponseDto(200, message: "Reserva eliminada.");
        }
    }
}