using Microsoft.EntityFrameworkCore;
using StayHub.Application.Ports.Out.Database;
using StayHub.Domain.Entities;
using StayHub.Domain.Enums;
using StayHub.Infrastructure.Out.Database.EfCore.Contexts;
using StayHub.Shared.Types;

namespace StayHub.Infrastructure.Out.Database.EfCore.Adapters;

/// <summary>
/// Implementación del repositorio de habitaciones usando EF Core
/// </summary>
public class HabitacionEfAdapter(StayHubDbContext context) : IHabitacionRepository
{
    public async Task<ResponseDb<Habitacion?>> GetByIdAsync(int habitacionId)
    {
        try
        {
            var habitacion = await context.Habitaciones
                .AsNoTracking()
                .Include(h => h.Hotel)
                .FirstOrDefaultAsync(h => h.HabitacionId == habitacionId);

            return new ResponseDb<Habitacion?>
            {
                Success = true,
                Message = habitacion != null ? "Habitación encontrada" : "Habitación no encontrada",
                Data = habitacion
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<Habitacion?>
            {
                Success = false,
                Message = $"Error al consultar habitación: {ex.Message}",
                ErrorCode = "GET_BY_ID_ERROR",
                Data = null
            };
        }
    }

    public async Task<ResponseDb<List<Habitacion>>> GetByHotelIdAsync(int hotelId)
    {
        try
        {
            var habitaciones = await context.Habitaciones
                .AsNoTracking()
                .Where(h => h.HotelId == hotelId)
                .OrderBy(h => h.NumeroHabitacion)
                .ToListAsync();

            return new ResponseDb<List<Habitacion>>
            {
                Success = true,
                Message = $"Consulta completada. Se encontraron {habitaciones.Count} habitaciones",
                Data = habitaciones
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<List<Habitacion>>
            {
                Success = false,
                Message = $"Error al consultar habitaciones del hotel: {ex.Message}",
                ErrorCode = "GET_BY_HOTEL_ERROR",
                Data = []
            };
        }
    }

    public async Task<ResponseDb<Pagination<Habitacion>>> GetPaginatedAsync(int pageNumber, int pageSize, int? hotelId = null)
    {
        try
        {
            var query = context.Habitaciones.AsNoTracking();

            if (hotelId.HasValue)
            {
                query = query.Where(h => h.HotelId == hotelId.Value);
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .Include(h => h.Hotel)
                .OrderBy(h => h.HotelId)
                .ThenBy(h => h.NumeroHabitacion)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagination = Pagination<Habitacion>.Create(items, totalRecords, pageNumber, pageSize);

            return new ResponseDb<Pagination<Habitacion>>
            {
                Success = true,
                Message = $"Consulta paginada completada. Página {pageNumber} de {pagination.TotalPages}, {totalRecords} registros totales",
                Data = pagination
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<Pagination<Habitacion>>
            {
                Success = false,
                Message = $"Error al consultar habitaciones paginadas: {ex.Message}",
                ErrorCode = "GET_PAGINATED_ERROR",
                Data = Pagination<Habitacion>.Create([], 0, pageNumber, pageSize)
            };
        }
    }

    public async Task<ResponseDb<Habitacion>> CreateAsync(Habitacion habitacion)
    {
        try
        {
            context.Habitaciones.Add(habitacion);
            await context.SaveChangesAsync();

            return new ResponseDb<Habitacion>
            {
                Success = true,
                Message = $"Habitación {habitacion.NumeroHabitacion} creada exitosamente con ID {habitacion.HabitacionId}",
                Data = habitacion
            };
        }
        catch (DbUpdateException ex)
        {
            return new ResponseDb<Habitacion>
            {
                Success = false,
                Message = $"Error de base de datos al crear habitación: {ex.InnerException?.Message ?? ex.Message}",
                ErrorCode = "DB_CREATE_ERROR"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<Habitacion>
            {
                Success = false,
                Message = $"Error inesperado al crear habitación: {ex.Message}",
                ErrorCode = "CREATE_ERROR"
            };
        }
    }

    public async Task<ResponseDb<Habitacion>> UpdateAsync(Habitacion habitacion)
    {
        try
        {
            var existing = await context.Habitaciones.FindAsync(habitacion.HabitacionId);
            if (existing == null)
            {
                return new ResponseDb<Habitacion>
                {
                    Success = false,
                    Message = $"Habitación con ID {habitacion.HabitacionId} no encontrada",
                    ErrorCode = "NOT_FOUND"
                };
            }

            existing.NumeroHabitacion = habitacion.NumeroHabitacion;
            existing.TipoHabitacion = habitacion.TipoHabitacion;
            existing.Capacidad = habitacion.Capacidad;
            existing.TarifaNoche = habitacion.TarifaNoche;

            await context.SaveChangesAsync();

            return new ResponseDb<Habitacion>
            {
                Success = true,
                Message = $"Habitación {existing.NumeroHabitacion} actualizada exitosamente",
                Data = existing
            };
        }
        catch (DbUpdateException ex)
        {
            return new ResponseDb<Habitacion>
            {
                Success = false,
                Message = $"Error de base de datos al actualizar habitación: {ex.InnerException?.Message ?? ex.Message}",
                ErrorCode = "DB_UPDATE_ERROR"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<Habitacion>
            {
                Success = false,
                Message = $"Error inesperado al actualizar habitación: {ex.Message}",
                ErrorCode = "UPDATE_ERROR"
            };
        }
    }

    public async Task<ResponseDb> SetEstadoAsync(int habitacionId, bool activo)
    {
        try
        {
            var habitacion = await context.Habitaciones.FindAsync(habitacionId);
            if (habitacion == null)
            {
                return new ResponseDb
                {
                    Success = false,
                    Message = $"Habitación con ID {habitacionId} no encontrada",
                    ErrorCode = "NOT_FOUND"
                };
            }

            var estadoAnterior = habitacion.Estado;
            habitacion.Estado = activo ? Estado.Activo : Estado.Inactivo;
            await context.SaveChangesAsync();

            return new ResponseDb
            {
                Success = true,
                Message = $"Habitación {habitacion.NumeroHabitacion} cambiada de {estadoAnterior} a {habitacion.Estado} exitosamente"
            };
        }
        catch (DbUpdateException ex)
        {
            return new ResponseDb
            {
                Success = false,
                Message = $"Error de base de datos al cambiar estado: {ex.InnerException?.Message ?? ex.Message}",
                ErrorCode = "DB_UPDATE_STATE_ERROR"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb
            {
                Success = false,
                Message = $"Error inesperado al cambiar estado: {ex.Message}",
                ErrorCode = "UPDATE_STATE_ERROR"
            };
        }
    }

    public async Task<ResponseDb<bool>> ExistsAsync(int habitacionId)
    {
        try
        {
            var exists = await context.Habitaciones.AnyAsync(h => h.HabitacionId == habitacionId);

            return new ResponseDb<bool>
            {
                Success = true,
                Message = exists ? "Habitación existe" : "Habitación no existe",
                Data = exists
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<bool>
            {
                Success = false,
                Message = $"Error al verificar existencia de habitación: {ex.Message}",
                ErrorCode = "EXISTS_CHECK_ERROR",
                Data = false
            };
        }
    }

    /// <summary>
    /// BR-04: Obtiene habitaciones activas disponibles para reserva
    /// - Solo habitaciones activas
    /// - Con capacidad suficiente para los huéspedes
    /// - Sin cruces de fechas con otras reservas activas
    /// </summary>
    public async Task<ResponseDb<List<Habitacion>>> GetDisponiblesAsync(int hotelId, DateTime fechaEntrada, DateTime fechaSalida, int cantidadHuespedes)
    {
        try
        {
            // Obtener IDs de habitaciones con reservas que se cruzan
            var habitacionesOcupadas = await context.Reservas
                .Where(r => r.HotelId == hotelId &&
                            r.EstadoReserva == EstadoReserva.Activa &&
                            r.FechaEntrada < fechaSalida &&
                            r.FechaSalida > fechaEntrada)
                .Select(r => r.HabitacionId)
                .Distinct()
                .ToListAsync();

            // Obtener habitaciones activas con capacidad, excluyendo las ocupadas
            var habitacionesDisponibles = await context.Habitaciones
                .AsNoTracking()
                .Where(h => h.HotelId == hotelId &&
                            h.Estado == Estado.Activo &&
                            h.Capacidad >= cantidadHuespedes &&
                            !habitacionesOcupadas.Contains(h.HabitacionId))
                .OrderBy(h => h.TarifaNoche)
                .ToListAsync();

            return new ResponseDb<List<Habitacion>>
            {
                Success = true,
                Message = $"Consulta de disponibilidad completada. {habitacionesDisponibles.Count} habitaciones disponibles de {fechaEntrada:yyyy-MM-dd} a {fechaSalida:yyyy-MM-dd} para {cantidadHuespedes} huéspedes",
                Data = habitacionesDisponibles
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<List<Habitacion>>
            {
                Success = false,
                Message = $"Error al consultar disponibilidad de habitaciones: {ex.Message}",
                ErrorCode = "GET_DISPONIBLES_ERROR",
                Data = []
            };
        }
    }
}
