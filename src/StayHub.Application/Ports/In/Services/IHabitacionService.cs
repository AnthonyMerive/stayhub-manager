using StayHub.Domain.Entities;
using StayHub.Shared.Types;

namespace StayHub.Application.Ports.In.Services;

/// <summary>
/// Puerto de entrada para operaciones de habitaciones
/// </summary>
public interface IHabitacionService
{
    Task<Habitacion?> GetByIdAsync(int habitacionId, string transactionId);
    Task<List<Habitacion>> GetByHotelIdAsync(int hotelId, string transactionId);
    Task<Pagination<Habitacion>> GetPaginatedAsync(int pageNumber, int pageSize, string transactionId, int? hotelId = null);

    /// <summary>
    /// Crea una nueva habitación
    /// Lanza BusinessRuleException si hay violación de reglas
    /// Lanza DatabaseException si hay error en repositorio
    /// </summary>
    Task<Habitacion> CreateAsync(Habitacion habitacion, string transactionId);

    /// <summary>
    /// Actualiza una habitación existente
    /// Lanza EntityNotFoundException si no existe la habitación
    /// Lanza DatabaseException si hay error en repositorio
    /// </summary>
    Task<Habitacion> UpdateAsync(Habitacion habitacion, string transactionId);

    /// <summary>
    /// Cambia el estado de una habitación (activo/inactivo)
    /// Lanza DatabaseException si no existe la habitación
    /// </summary>
    Task SetEstadoAsync(int habitacionId, bool activo, string transactionId);

    /// <summary>
    /// Consulta de disponibilidad (BR-04: solo habitaciones activas)
    /// </summary>
    Task<List<Habitacion>> GetDisponiblesAsync(int hotelId, DateTime fechaEntrada, DateTime fechaSalida, int cantidadHuespedes, string transactionId);
}
