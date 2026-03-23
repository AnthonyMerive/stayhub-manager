using StayHub.Domain.Entities;
using StayHub.Shared.Types;

namespace StayHub.Application.Ports.Out.Database;

/// <summary>
/// Puerto de salida para operaciones de persistencia de habitaciones
/// </summary>
public interface IHabitacionRepository
{
    /// <summary>
    /// Obtiene una habitación por su ID
    /// </summary>
    Task<ResponseDb<Habitacion?>> GetByIdAsync(int habitacionId);

    /// <summary>
    /// Obtiene todas las habitaciones de un hotel
    /// </summary>
    Task<ResponseDb<List<Habitacion>>> GetByHotelIdAsync(int hotelId);

    /// <summary>
    /// Obtiene habitaciones paginadas con filtro opcional por hotel
    /// </summary>
    Task<ResponseDb<Pagination<Habitacion>>> GetPaginatedAsync(int pageNumber, int pageSize, int? hotelId = null);

    /// <summary>
    /// Crea una nueva habitación
    /// </summary>
    Task<ResponseDb<Habitacion>> CreateAsync(Habitacion habitacion);

    /// <summary>
    /// Actualiza una habitación existente
    /// </summary>
    Task<ResponseDb<Habitacion>> UpdateAsync(Habitacion habitacion);

    /// <summary>
    /// Cambia el estado de una habitación (activo/inactivo)
    /// </summary>
    Task<ResponseDb> SetEstadoAsync(int habitacionId, bool activo);

    /// <summary>
    /// Verifica si existe una habitación con el ID especificado
    /// </summary>
    Task<ResponseDb<bool>> ExistsAsync(int habitacionId);

    /// <summary>
    /// BR-04: Obtiene habitaciones activas disponibles para reserva
    /// </summary>
    Task<ResponseDb<List<Habitacion>>> GetDisponiblesAsync(int hotelId, DateTime fechaEntrada, DateTime fechaSalida, int cantidadHuespedes);
}
