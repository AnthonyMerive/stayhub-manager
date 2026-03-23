using Microsoft.EntityFrameworkCore;
using StayHub.Application.Ports.Out.Database;
using StayHub.Domain.Entities;
using StayHub.Domain.Enums;
using StayHub.Infrastructure.Out.Database.EfCore.Contexts;
using StayHub.Shared.Types;

namespace StayHub.Infrastructure.Out.Database.EfCore.Adapters;

/// <summary>
/// Implementación del repositorio de hoteles usando EF Core
/// </summary>
public class HotelEfAdapter(StayHubDbContext context) : IHotelRepository
{
    public async Task<ResponseDb<Hotel?>> GetByIdAsync(int hotelId)
    {
        try
        {
            var hotel = await context.Hoteles
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.HotelId == hotelId);

            return new ResponseDb<Hotel?>
            {
                Success = true,
                Message = hotel != null ? "Hotel encontrado" : "Hotel no encontrado",
                Data = hotel
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<Hotel?>
            {
                Success = false,
                Message = $"Error al consultar hotel: {ex.Message}",
                ErrorCode = "GET_BY_ID_ERROR",
                Data = null
            };
        }
    }

    public async Task<ResponseDb<List<Hotel>>> GetAllAsync()
    {
        try
        {
            var hoteles = await context.Hoteles
                .AsNoTracking()
                .OrderBy(h => h.Nombre)
                .ToListAsync();

            return new ResponseDb<List<Hotel>>
            {
                Success = true,
                Message = $"Consulta completada. Se encontraron {hoteles.Count} hoteles",
                Data = hoteles
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<List<Hotel>>
            {
                Success = false,
                Message = $"Error al consultar todos los hoteles: {ex.Message}",
                ErrorCode = "GET_ALL_ERROR",
                Data = []
            };
        }
    }

    public async Task<ResponseDb<Pagination<Hotel>>> GetPaginatedAsync(int pageNumber, int pageSize, string? searchTerm = null)
    {
        try
        {
            var query = context.Hoteles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(h => 
                    h.Nombre.Contains(searchTerm) || 
                    h.Ciudad.Contains(searchTerm));
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderBy(h => h.Nombre)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagination = Pagination<Hotel>.Create(items, totalRecords, pageNumber, pageSize);

            return new ResponseDb<Pagination<Hotel>>
            {
                Success = true,
                Message = $"Consulta paginada completada. Página {pageNumber} de {pagination.TotalPages}, {totalRecords} registros totales. Búsqueda: '{searchTerm ?? "sin filtro"}'",
                Data = pagination
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<Pagination<Hotel>>
            {
                Success = false,
                Message = $"Error al consultar hoteles paginados: {ex.Message}",
                ErrorCode = "GET_PAGINATED_ERROR",
                Data = Pagination<Hotel>.Create([], 0, pageNumber, pageSize)
            };
        }
    }

    public async Task<ResponseDb<Hotel>> CreateAsync(Hotel hotel)
    {
        try
        {
            context.Hoteles.Add(hotel);
            await context.SaveChangesAsync();

            return new ResponseDb<Hotel>
            {
                Success = true,
                Message = $"Hotel '{hotel.Nombre}' creado exitosamente con ID {hotel.HotelId}",
                Data = hotel
            };
        }
        catch (DbUpdateException ex)
        {
            return new ResponseDb<Hotel>
            {
                Success = false,
                Message = $"Error de base de datos al crear hotel: {ex.InnerException?.Message ?? ex.Message}",
                ErrorCode = "DB_CREATE_ERROR"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<Hotel>
            {
                Success = false,
                Message = $"Error inesperado al crear hotel: {ex.Message}",
                ErrorCode = "CREATE_ERROR"
            };
        }
    }

    public async Task<ResponseDb<Hotel>> UpdateAsync(Hotel hotel)
    {
        try
        {
            var existing = await context.Hoteles.FindAsync(hotel.HotelId);
            if (existing == null)
            {
                return new ResponseDb<Hotel>
                {
                    Success = false,
                    Message = $"Hotel con ID {hotel.HotelId} no encontrado",
                    ErrorCode = "NOT_FOUND"
                };
            }

            existing.Nombre = hotel.Nombre;
            existing.Ciudad = hotel.Ciudad;
            existing.Direccion = hotel.Direccion;

            await context.SaveChangesAsync();

            return new ResponseDb<Hotel>
            {
                Success = true,
                Message = $"Hotel '{existing.Nombre}' actualizado exitosamente",
                Data = existing
            };
        }
        catch (DbUpdateException ex)
        {
            return new ResponseDb<Hotel>
            {
                Success = false,
                Message = $"Error de base de datos al actualizar hotel: {ex.InnerException?.Message ?? ex.Message}",
                ErrorCode = "DB_UPDATE_ERROR"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<Hotel>
            {
                Success = false,
                Message = $"Error inesperado al actualizar hotel: {ex.Message}",
                ErrorCode = "UPDATE_ERROR"
            };
        }
    }

    public async Task<ResponseDb> SetEstadoAsync(int hotelId, bool activo)
    {
        try
        {
            var hotel = await context.Hoteles.FindAsync(hotelId);
            if (hotel == null)
            {
                return new ResponseDb
                {
                    Success = false,
                    Message = $"Hotel con ID {hotelId} no encontrado",
                    ErrorCode = "NOT_FOUND"
                };
            }

            var estadoAnterior = hotel.Estado;
            hotel.Estado = activo ? Estado.Activo : Estado.Inactivo;
            await context.SaveChangesAsync();

            return new ResponseDb
            {
                Success = true,
                Message = $"Hotel '{hotel.Nombre}' cambiado de {estadoAnterior} a {hotel.Estado} exitosamente"
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

    public async Task<ResponseDb<bool>> ExistsAsync(int hotelId)
    {
        try
        {
            var exists = await context.Hoteles.AnyAsync(h => h.HotelId == hotelId);

            return new ResponseDb<bool>
            {
                Success = true,
                Message = exists ? "Hotel existe" : "Hotel no existe",
                Data = exists
            };
        }
        catch (Exception ex)
        {
            return new ResponseDb<bool>
            {
                Success = false,
                Message = $"Error al verificar existencia de hotel: {ex.Message}",
                ErrorCode = "EXISTS_CHECK_ERROR",
                Data = false
            };
        }
    }
}
