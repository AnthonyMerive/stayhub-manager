using StayHub.Domain.Entities;
using StayHub.Shared.Types;

namespace StayHub.Application.Ports.In.Services;

/// <summary>
/// Puerto de entrada para operaciones de habitaciones
/// </summary>
public interface IHabitacionService
{
    /// <summary>
    /// Obtiene una habitación por su identificador único
    /// </summary>
    /// <param name="habitacionId">Identificador único de la habitación</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>La habitación encontrada o null si no existe</returns>
    /// <exception cref="BusinessException">Si el ID de la habitación es inválido</exception>
    /// <exception cref="DatabaseException">Si ocurre un error en la base de datos</exception>
    Task<Habitacion?> GetByIdAsync(int habitacionId, string transactionId);

    /// <summary>
    /// Obtiene todas las habitaciones de un hotel específico
    /// </summary>
    /// <param name="hotelId">Identificador único del hotel</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>Lista de habitaciones del hotel</returns>
    /// <exception cref="BusinessException">Si el ID del hotel es inválido</exception>
    /// <exception cref="DatabaseException">Si ocurre un error en la base de datos</exception>
    Task<List<Habitacion>> GetByHotelIdAsync(int hotelId, string transactionId);

    /// <summary>
    /// Obtiene habitaciones paginadas con filtro opcional por hotel
    /// </summary>
    /// <param name="pageNumber">Número de página (basado en 1)</param>
    /// <param name="pageSize">Tamaño de la página</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <param name="hotelId">Identificador opcional del hotel para filtrar</param>
    /// <returns>Objeto de paginación con las habitaciones encontradas</returns>
    /// <exception cref="BusinessException">Si los parámetros de paginación o el ID del hotel son inválidos</exception>
    /// <exception cref="DatabaseException">Si ocurre un error en la base de datos</exception>
    Task<Pagination<Habitacion>> GetPaginatedAsync(int pageNumber, int pageSize, string transactionId, int? hotelId = null);

    /// <summary>
    /// Crea una nueva habitación en el sistema
    /// </summary>
    /// <param name="habitacion">Datos de la habitación a crear</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>La habitación creada con su ID asignado</returns>
    /// <exception cref="BusinessException">Si hay violación de reglas de negocio</exception>
    /// <exception cref="DatabaseException">Si hay error en repositorio</exception>
    Task<Habitacion> CreateAsync(Habitacion habitacion, string transactionId);

    /// <summary>
    /// Actualiza una habitación existente
    /// </summary>
    /// <param name="habitacion">Datos de la habitación a actualizar (debe incluir ID)</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>La habitación actualizada</returns>
    /// <exception cref="BusinessException">Si hay violación de reglas de negocio</exception>
    /// <exception cref="NotFoundException">Si no existe la habitación</exception>
    /// <exception cref="DatabaseException">Si hay error en repositorio</exception>
    Task<Habitacion> UpdateAsync(Habitacion habitacion, string transactionId);

    /// <summary>
    /// Cambia el estado de una habitación (activo/inactivo)
    /// </summary>
    /// <param name="habitacionId">Identificador único de la habitación</param>
    /// <param name="activo">Nuevo estado de la habitación (true = activa, false = inactiva)</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <exception cref="BusinessException">Si el ID de la habitación es inválido</exception>
    /// <exception cref="NotFoundException">Si no existe la habitación</exception>
    /// <exception cref="DatabaseException">Si hay error en repositorio</exception>
    Task SetEstadoAsync(int habitacionId, bool activo, string transactionId);

    /// <summary>
    /// Consulta habitaciones disponibles para reserva (BR-04: solo habitaciones activas)
    /// </summary>
    /// <param name="hotelId">Identificador único del hotel</param>
    /// <param name="fechaEntrada">Fecha de entrada de la reserva</param>
    /// <param name="fechaSalida">Fecha de salida de la reserva</param>
    /// <param name="cantidadHuespedes">Cantidad de huéspedes para la reserva</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>Lista de habitaciones disponibles que cumplen los criterios</returns>
    /// <exception cref="BusinessException">Si los parámetros de búsqueda son inválidos</exception>
    /// <exception cref="DatabaseException">Si ocurre un error en la base de datos</exception>
    Task<List<Habitacion>> GetDisponiblesAsync(int hotelId, DateTime fechaEntrada, DateTime fechaSalida, int cantidadHuespedes, string transactionId);
}
