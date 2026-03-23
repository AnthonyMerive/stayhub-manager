using StayHub.Domain.Entities;
using StayHub.Shared.Types;

namespace StayHub.Application.Ports.Out.Database;

/// <summary>
/// Puerto de salida para operaciones de persistencia de hoteles
/// </summary>
public interface IHotelRepository
{
    /// <summary>
    /// Obtiene un hotel por su ID
    /// </summary>
    Task<ResponseDb<Hotel?>> GetByIdAsync(int hotelId);

    /// <summary>
    /// Obtiene todos los hoteles
    /// </summary>
    Task<ResponseDb<List<Hotel>>> GetAllAsync();

    /// <summary>
    /// Obtiene hoteles paginados con filtro opcional de búsqueda
    /// </summary>
    Task<ResponseDb<Pagination<Hotel>>> GetPaginatedAsync(int pageNumber, int pageSize, string? searchTerm = null);

    /// <summary>
    /// Crea un nuevo hotel
    /// </summary>
    Task<ResponseDb<Hotel>> CreateAsync(Hotel hotel);

    /// <summary>
    /// Actualiza un hotel existente
    /// </summary>
    Task<ResponseDb<Hotel>> UpdateAsync(Hotel hotel);

    /// <summary>
    /// Cambia el estado de un hotel (activo/inactivo)
    /// </summary>
    Task<ResponseDb> SetEstadoAsync(int hotelId, bool activo);

    /// <summary>
    /// Verifica si existe un hotel con el ID especificado
    /// </summary>
    Task<ResponseDb<bool>> ExistsAsync(int hotelId);
}
