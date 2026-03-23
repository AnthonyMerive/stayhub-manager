using Microsoft.EntityFrameworkCore;
using StayHub.Application.Ports.Out.Database;
using StayHub.Domain.Entities;
using StayHub.Domain.Enums;
using StayHub.Infrastructure.Out.Database.EfCore.Contexts;
using StayHub.Shared.Types;

namespace StayHub.Infrastructure.Out.Database.EfCore.Adapters;

/// <summary>
/// Implementación del repositorio de reservas usando EF Core
/// </summary>
public class ReservaEfAdapter(StayHubDbContext context) : IReservaRepository
{
    public async Task<ResponseDb<Reserva?>> GetByIdAsync(int reservaId)
    {
        try
        {
            var reserva = await context.Reservas
                .AsNoTracking()
                .Include(r => r.Hotel)
                .Include(r => r.Habitacion)
                .FirstOrDefaultAsync(r => r.ReservaId == reservaId);

            return new ResponseDb<Reserva?>
            {
                Success = true,
                Message = reserva != null ? "Reserva encontrada" : "Reserva no encontrada",
                Data = reserva
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<Reserva?>
            {
                Success = false,
                Message = $"Error al consultar reserva: {ex.Message}",
                ErrorCode = "GET_BY_ID_ERROR",
                Data = null
            };
        }
    }

    public async Task<ResponseDb<List<Reserva>>> GetByHotelIdAsync(int hotelId)
    {
        try
        {
            var reservas = await context.Reservas
                .AsNoTracking()
                .Include(r => r.Habitacion)
                .Where(r => r.HotelId == hotelId)
                .OrderByDescending(r => r.FechaCreacion)
                .ToListAsync();

            return new ResponseDb<List<Reserva>>
            {
                Success = true,
                Message = $"Consulta completada. Se encontraron {reservas.Count} reservas para el hotel",
                Data = reservas
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<List<Reserva>>
            {
                Success = false,
                Message = $"Error al consultar reservas del hotel: {ex.Message}",
                ErrorCode = "GET_BY_HOTEL_ERROR",
                Data = []
            };
        }
    }

    public async Task<ResponseDb<List<Reserva>>> GetByHabitacionIdAsync(int habitacionId)
    {
        try
        {
            var reservas = await context.Reservas
                .AsNoTracking()
                .Where(r => r.HabitacionId == habitacionId)
                .OrderByDescending(r => r.FechaCreacion)
                .ToListAsync();

            return new ResponseDb<List<Reserva>>
            {
                Success = true,
                Message = $"Consulta completada. Se encontraron {reservas.Count} reservas para la habitación",
                Data = reservas
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<List<Reserva>>
            {
                Success = false,
                Message = $"Error al consultar reservas de la habitación: {ex.Message}",
                ErrorCode = "GET_BY_HABITACION_ERROR",
                Data = []
            };
        }
    }

    public async Task<ResponseDb<Pagination<Reserva>>> GetPaginatedAsync(int pageNumber, int pageSize, int? hotelId = null, int? habitacionId = null)
    {
        try
        {
            var query = context.Reservas.AsNoTracking();

            if (hotelId.HasValue)
            {
                query = query.Where(r => r.HotelId == hotelId.Value);
            }

            if (habitacionId.HasValue)
            {
                query = query.Where(r => r.HabitacionId == habitacionId.Value);
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .Include(r => r.Hotel)
                .Include(r => r.Habitacion)
                .OrderByDescending(r => r.FechaCreacion)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagination = Pagination<Reserva>.Create(items, totalRecords, pageNumber, pageSize);

            return new ResponseDb<Pagination<Reserva>>
            {
                Success = true,
                Message = $"Consulta paginada completada. Página {pageNumber} de {pagination.TotalPages}, {totalRecords} registros totales. Filtros: Hotel={hotelId?.ToString() ?? "todos"}, Habitación={habitacionId?.ToString() ?? "todas"}",
                Data = pagination
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<Pagination<Reserva>>
            {
                Success = false,
                Message = $"Error al consultar reservas paginadas: {ex.Message}",
                ErrorCode = "GET_PAGINATED_ERROR",
                Data = Pagination<Reserva>.Create([], 0, pageNumber, pageSize)
            };
        }
    }

    public async Task<ResponseDb<Reserva>> CreateAsync(Reserva reserva)
    {
        try
        {
            context.Reservas.Add(reserva);
            await context.SaveChangesAsync();

            return new ResponseDb<Reserva>
            {
                Success = true,
                Message = $"Reserva creada exitosamente para {reserva.HuespedNombre} del {reserva.FechaEntrada:yyyy-MM-dd} al {reserva.FechaSalida:yyyy-MM-dd}",
                Data = reserva
            };
        }
        catch (DbUpdateException ex)
        {
            return new ResponseDb<Reserva>
            {
                Success = false,
                Message = $"Error de base de datos al crear reserva: {ex.InnerException?.Message ?? ex.Message}",
                ErrorCode = "DB_CREATE_ERROR"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<Reserva>
            {
                Success = false,
                Message = $"Error inesperado al crear reserva: {ex.Message}",
                ErrorCode = "CREATE_ERROR"
            };
        }
    }

    public async Task<ResponseDb<Reserva>> UpdateAsync(Reserva reserva)
    {
        try
        {
            var existing = await context.Reservas.FindAsync(reserva.ReservaId);
            if (existing == null)
            {
                return new ResponseDb<Reserva>
                {
                    Success = false,
                    Message = $"Reserva con ID {reserva.ReservaId} no encontrada",
                    ErrorCode = "NOT_FOUND"
                };
            }

            existing.HuespedNombre = reserva.HuespedNombre;
            existing.HuespedDocumento = reserva.HuespedDocumento;
            existing.FechaEntrada = reserva.FechaEntrada;
            existing.FechaSalida = reserva.FechaSalida;
            existing.CantidadHuespedes = reserva.CantidadHuespedes;

            await context.SaveChangesAsync();

            return new ResponseDb<Reserva>
            {
                Success = true,
                Message = $"Reserva actualizada exitosamente para {existing.HuespedNombre}",
                Data = existing
            };
        }
        catch (DbUpdateException ex)
        {
            return new ResponseDb<Reserva>
            {
                Success = false,
                Message = $"Error de base de datos al actualizar reserva: {ex.InnerException?.Message ?? ex.Message}",
                ErrorCode = "DB_UPDATE_ERROR"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<Reserva>
            {
                Success = false,
                Message = $"Error inesperado al actualizar reserva: {ex.Message}",
                ErrorCode = "UPDATE_ERROR"
            };
        }
    }

    /// <summary>
    /// BR-06: Cancelación lógica (no elimina registros de la base de datos)
    /// </summary>
    public async Task<ResponseDb> CancelarAsync(int reservaId)
    {
        try
        {
            var reserva = await context.Reservas.FindAsync(reservaId);
            if (reserva == null)
            {
                return new ResponseDb
                {
                    Success = false,
                    Message = $"Reserva con ID {reservaId} no encontrada",
                    ErrorCode = "NOT_FOUND"
                };
            }

            if (reserva.EstadoReserva == EstadoReserva.Cancelada)
            {
                return new ResponseDb
                {
                    Success = false,
                    Message = "La reserva ya se encuentra cancelada",
                    ErrorCode = "ALREADY_CANCELLED"
                };
            }

            var estadoAnterior = reserva.EstadoReserva;
            reserva.EstadoReserva = EstadoReserva.Cancelada;
            await context.SaveChangesAsync();

            return new ResponseDb
            {
                Success = true,
                Message = $"Reserva de {reserva.HuespedNombre} cancelada exitosamente (era {estadoAnterior})"
            };
        }
        catch (DbUpdateException ex)
        {
            return new ResponseDb
            {
                Success = false,
                Message = $"Error de base de datos al cancelar reserva: {ex.InnerException?.Message ?? ex.Message}",
                ErrorCode = "DB_CANCEL_ERROR"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb
            {
                Success = false,
                Message = $"Error inesperado al cancelar reserva: {ex.Message}",
                ErrorCode = "CANCEL_ERROR"
            };
        }
    }

    /// <summary>
    /// BR-02: Verifica si existe cruce de fechas para una habitación
    /// Una reserva se cruza si su rango de fechas se solapa con el rango solicitado
    /// </summary>
    public async Task<ResponseDb<bool>> ExisteOverbookingAsync(int habitacionId, DateTime fechaEntrada, DateTime fechaSalida, int? excludeReservaId = null)
    {
        try
        {
            var query = context.Reservas
                .Where(r => r.HabitacionId == habitacionId &&
                            r.EstadoReserva == EstadoReserva.Activa &&
                            r.FechaEntrada < fechaSalida &&
                            r.FechaSalida > fechaEntrada);

            if (excludeReservaId.HasValue)
            {
                query = query.Where(r => r.ReservaId != excludeReservaId.Value);
            }

            var existe = await query.AnyAsync();

            return new ResponseDb<bool>
            {
                Success = true,
                Message = existe ? $"Existe conflicto de fechas para habitación {habitacionId}" : $"No existe conflicto de fechas para habitación {habitacionId}",
                Data = existe
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<bool>
            {
                Success = false,
                Message = $"Error al verificar overbooking: {ex.Message}",
                ErrorCode = "OVERBOOKING_CHECK_ERROR",
                Data = false
            };
        }
    }

    public async Task<ResponseDb<bool>> ExistsAsync(int reservaId)
    {
        try
        {
            var exists = await context.Reservas.AnyAsync(r => r.ReservaId == reservaId);

            return new ResponseDb<bool>
            {
                Success = true,
                Message = exists ? "Reserva existe" : "Reserva no existe",
                Data = exists
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<bool>
            {
                Success = false,
                Message = $"Error al verificar existencia de reserva: {ex.Message}",
                ErrorCode = "EXISTS_CHECK_ERROR",
                Data = false
            };
        }
    }
}
