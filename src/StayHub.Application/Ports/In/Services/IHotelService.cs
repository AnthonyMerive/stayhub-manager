using StayHub.Domain.Entities;
using StayHub.Shared.Types;

namespace StayHub.Application.Ports.In.Services;

/// <summary>
/// Puerto de entrada para operaciones de hoteles
/// </summary>
public interface IHotelService
{
    Task<Hotel?> GetByIdAsync(int hotelId, string transactionId);
    Task<List<Hotel>> GetAllAsync(string transactionId);
    Task<Pagination<Hotel>> GetPaginatedAsync(int pageNumber, int pageSize, string transactionId, string? searchTerm = null);

    /// <summary>
    /// Crea un nuevo hotel
    /// Lanza BusinessRuleException si hay violación de reglas
    /// Lanza DatabaseException si hay error en repositorio
    /// </summary>
    Task<Hotel> CreateAsync(Hotel hotel, string transactionId);

    /// <summary>
    /// Actualiza un hotel existente
    /// Lanza EntityNotFoundException si no existe el hotel
    /// Lanza DatabaseException si hay error en repositorio
    /// </summary>
    Task<Hotel> UpdateAsync(Hotel hotel, string transactionId);

    /// <summary>
    /// Cambia el estado de un hotel (activo/inactivo)
    /// Lanza EntityNotFoundException si no existe el hotel
    /// </summary>
    Task SetEstadoAsync(int hotelId, bool activo, string transactionId);
}
