using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Context;
using VPASS3_backend.DTOs;
using VPASS3_backend.DTOs.CommonAreas;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Interfaces.CommonAreaInterfaces;
using VPASS3_backend.Models.CommonAreas.ReservableCommonArea;
using VPASS3_backend.Utils;

namespace VPASS3_backend.Services.CommonAreaServices
{
    public class ReservableCommonAreaReservationService : IReservableCommonAreaReservationService
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public ReservableCommonAreaReservationService(AppDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<ResponseDto> CreateAsync(CreateReservableCommonAreaReservationDto dto)
        {
            try
            {
                var area = await _context.CommonAreas
                    .FirstOrDefaultAsync(ca => ca.Id == dto.IdReservableCommonArea);

                if (area == null || area.Type != Enums.CommonAreaType.Reservable)
                    return new ResponseDto(404, message: "Área común reservable no encontrada o no válida.");

                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != area.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No tienes permisos para reservar en este establecimiento.");
                }

                var person = await _context.Persons.FindAsync(dto.IdPersonReservedBy);
                if (person == null)
                    return new ResponseDto(404, message: "Persona que reserva no encontrada.");

                var reservation = new CommonAreaReservation
                {
                    IdReservableCommonArea = dto.IdReservableCommonArea,
                    IdPersonReservedBy = dto.IdPersonReservedBy,
                    ReservationTime = dto.ReservationTime,
                    ReservationStart = TimeHelper.GetSantiagoTime(),
                };

                _context.CommonAreaReservations.Add(reservation);
                await _context.SaveChangesAsync();

                return new ResponseDto(201, reservation, "Reserva creada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CreateAsync CommonAreaReservationService: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al crear la reserva.");
            }
        }
        public async Task<ResponseDto> GetAllAsync()
        {
            try
            {
                IQueryable<CommonAreaReservation> query = _context.CommonAreaReservations;

                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "No tienes un establecimiento asociado.");

                    query = query.Where(r =>
                        r.ReservableCommonArea.IdEstablishment == _userContext.EstablishmentId);
                }

                var data = await query
                    .Include(r => r.ReservedBy)
                    .Include(r => r.ReservableCommonArea)
                    .ToListAsync();

                return new ResponseDto(200, data, "Reservas obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetAllAsync CommonAreaReservationService: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al obtener las reservas.");
            }
        }

        public async Task<ResponseDto> GetByIdAsync(int id)
        {
            try
            {
                var reservation = await _context.CommonAreaReservations
                    .Include(r => r.ReservedBy)
                    .Include(r => r.ReservableCommonArea)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reservation == null)
                    return new ResponseDto(404, message: "Reserva no encontrada.");

                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != reservation.ReservableCommonArea.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No tienes permisos para acceder a esta reserva.");
                }

                return new ResponseDto(200, reservation, "Reserva obtenida correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetByIdAsync CommonAreaReservationService: " + ex.Message);
                return new ResponseDto(500, message: "Error al obtener la reserva.");
            }
        }

        public async Task<ResponseDto> UpdateAsync(int id, CreateReservableCommonAreaReservationDto dto)
        {
            try
            {
                var reservation = await _context.CommonAreaReservations
                    .Include(r => r.ReservableCommonArea)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reservation == null)
                    return new ResponseDto(404, message: "Reserva no encontrada.");

                // Validación de establecimiento
                if (_userContext.UserRole != "SUPERADMIN")
                {
                    if (!_userContext.EstablishmentId.HasValue)
                        return new ResponseDto(403, message: "No tienes un establecimiento asociado.");

                    if (reservation.ReservableCommonArea.IdEstablishment != _userContext.EstablishmentId.Value)
                        return new ResponseDto(403, message: "No tienes permisos para editar esta reserva.");
                }

                // Validar que el área exista y sea de tipo RESERVABLE
                var commonArea = await _context.CommonAreas
                    .OfType<ReservableCommonArea>()
                    .FirstOrDefaultAsync(ca => ca.Id == dto.IdReservableCommonArea);

                if (commonArea == null)
                    return new ResponseDto(404, message: "Área común reservable no encontrada.");

                // Validar persona
                var person = await _context.Persons.FindAsync(dto.IdPersonReservedBy);
                if (person == null)
                    return new ResponseDto(404, message: "Persona reservante no encontrada.");

                // Actualizar datos
                reservation.ReservationTime = dto.ReservationTime;
                reservation.IdPersonReservedBy = dto.IdPersonReservedBy;
                reservation.IdReservableCommonArea = dto.IdReservableCommonArea;

                return new ResponseDto(200, reservation, "Reserva actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en UpdateAsync CommonAreaReservationService: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al actualizar la reserva.");
            }
        }


        public async Task<ResponseDto> DeleteAsync(int id)
        {
            try
            {
                var reservation = await _context.CommonAreaReservations
                    .Include(r => r.ReservableCommonArea)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reservation == null)
                    return new ResponseDto(404, message: "Reserva no encontrada.");

                if (_userContext.UserRole != "SUPERADMIN" &&
                    _userContext.EstablishmentId != reservation.ReservableCommonArea.IdEstablishment)
                {
                    return new ResponseDto(403, message: "No tienes permisos para eliminar esta reserva.");
                }

                _context.CommonAreaReservations.Remove(reservation);
                await _context.SaveChangesAsync();

                return new ResponseDto(200, message: "Reserva eliminada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en DeleteAsync CommonAreaReservationService: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor al eliminar la reserva.");
            }
        }
    }
}