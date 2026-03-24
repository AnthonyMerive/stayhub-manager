using StayHub.Domain.Entities;
using StayHub.Shared.Types;

namespace StayHub.Application.Ports.In.Services;

/// <summary>
/// Puerto de entrada para operaciones de reservas
/// Implementa las reglas de negocio BR-01 a BR-06
/// </summary>
public interface IReservaService
{
    /// <summary>
    /// Obtiene una reserva por su identificador único
    /// </summary>
    /// <param name="reservaId">Identificador único de la reserva</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>La reserva encontrada o null si no existe</returns>
    /// <exception cref="BusinessException">Si el ID de la reserva es inválido</exception>
    /// <exception cref="DatabaseException">Si ocurre un error en la base de datos</exception>
    Task<Reserva?> GetByIdAsync(int reservaId, string transactionId);

    /// <summary>
    /// Obtiene todas las reservas de un hotel específico
    /// </summary>
    /// <param name="hotelId">Identificador único del hotel</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>Lista de reservas del hotel</returns>
    /// <exception cref="BusinessException">Si el ID del hotel es inválido</exception>
    /// <exception cref="DatabaseException">Si ocurre un error en la base de datos</exception>
    Task<List<Reserva>> GetByHotelIdAsync(int hotelId, string transactionId);

    /// <summary>
    /// Obtiene reservas paginadas con filtros opcionales por hotel y habitación
    /// </summary>
    /// <param name="pageNumber">Número de página (basado en 1)</param>
    /// <param name="pageSize">Tamaño de la página</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <param name="hotelId">Identificador opcional del hotel para filtrar</param>
    /// <param name="habitacionId">Identificador opcional de la habitación para filtrar</param>
    /// <returns>Objeto de paginación con las reservas encontradas</returns>
    /// <exception cref="BusinessException">Si los parámetros de paginación son inválidos</exception>
    /// <exception cref="DatabaseException">Si ocurre un error en la base de datos</exception>
    Task<Pagination<Reserva>> GetPaginatedAsync(int pageNumber, int pageSize, string transactionId, int? hotelId = null, int? habitacionId = null);

    /// <summary>
    /// Crea una reserva aplicando todas las reglas de negocio:
    /// BR-01: Validación de fechas (entrada < salida, no fechas pasadas)
    /// BR-02: Prevención de overbooking (verificación de disponibilidad)
    /// BR-03: Validación de capacidad de huéspedes
    /// BR-04: Solo habitaciones activas disponibles
    /// BR-05: Cálculo automático del total basado en tarifa y noches
    /// </summary>
    /// <param name="reserva">Datos de la reserva a crear</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>La reserva creada con su ID asignado y total calculado</returns>
    /// <exception cref="BusinessException">Si se viola alguna regla de negocio</exception>
    /// <exception cref="NotFoundException">Si no se encuentra hotel o habitación</exception>
    /// <exception cref="DatabaseException">Si hay error en repositorio</exception>
    Task<Reserva> CreateAsync(Reserva reserva, string transactionId);

    /// <summary>
    /// Cancelación lógica de reserva (BR-06: no elimina registros, solo cambia estado)
    /// </summary>
    /// <param name="reservaId">Identificador único de la reserva a cancelar</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <exception cref="BusinessException">Si el ID es inválido o la reserva ya está cancelada</exception>
    /// <exception cref="NotFoundException">Si no se encuentra la reserva</exception>
    /// <exception cref="DatabaseException">Si hay error en repositorio</exception>
    Task CancelarAsync(int reservaId, string transactionId);
}
