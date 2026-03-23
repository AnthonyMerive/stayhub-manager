using StayHub.Domain.Entities;
using StayHub.Shared.Types;

namespace StayHub.Application.Ports.Out.Database;

/// <summary>
/// Puerto de salida para operaciones de persistencia de reservas
/// </summary>
public interface IReservaRepository
{
    /// <summary>
    /// Obtiene una reserva por su ID
    /// </summary>
    Task<ResponseDb<Reserva?>> GetByIdAsync(int reservaId);

    /// <summary>
    /// Obtiene todas las reservas de un hotel
    /// </summary>
    Task<ResponseDb<List<Reserva>>> GetByHotelIdAsync(int hotelId);

    /// <summary>
    /// Obtiene todas las reservas de una habitación
    /// </summary>
    Task<ResponseDb<List<Reserva>>> GetByHabitacionIdAsync(int habitacionId);

    /// <summary>
    /// Obtiene reservas paginadas con filtros opcionales
    /// </summary>
    Task<ResponseDb<Pagination<Reserva>>> GetPaginatedAsync(int pageNumber, int pageSize, int? hotelId = null, int? habitacionId = null);

    /// <summary>
    /// Crea una nueva reserva
    /// </summary>
    Task<ResponseDb<Reserva>> CreateAsync(Reserva reserva);

    /// <summary>
    /// Actualiza una reserva existente
    /// </summary>
    Task<ResponseDb<Reserva>> UpdateAsync(Reserva reserva);

    /// <summary>
    /// BR-06: Cancelación lógica de la reserva (no elimina registros)
    /// </summary>
    Task<ResponseDb> CancelarAsync(int reservaId);

    /// <summary>
    /// BR-02: Verifica si existe cruce de fechas para una habitación
    /// </summary>
    Task<ResponseDb<bool>> ExisteOverbookingAsync(int habitacionId, DateTime fechaEntrada, DateTime fechaSalida, int? excludeReservaId = null);

    /// <summary>
    /// Verifica si existe una reserva con el ID especificado
    /// </summary>
    Task<ResponseDb<bool>> ExistsAsync(int reservaId);
}
