using StayHub.Domain.Entities;
using StayHub.Shared.Types;

namespace StayHub.Application.Ports.In.Services;

/// <summary>
/// Puerto de entrada para operaciones de reservas
/// Implementa las reglas de negocio BR-01 a BR-06
/// </summary>
public interface IReservaService
{
    Task<Reserva?> GetByIdAsync(int reservaId, string transactionId);
    Task<List<Reserva>> GetByHotelIdAsync(int hotelId, string transactionId);
    Task<Pagination<Reserva>> GetPaginatedAsync(int pageNumber, int pageSize, string transactionId, int? hotelId = null, int? habitacionId = null);

    /// <summary>
    /// Crea una reserva aplicando todas las reglas de negocio:
    /// BR-01: Validación de fechas
    /// BR-02: Prevención de overbooking
    /// BR-03: Validación de capacidad
    /// BR-04: Solo habitaciones activas
    /// BR-05: Cálculo automático del total
    /// 
    /// Lanza BusinessRuleException si se viola alguna regla de negocio
    /// Lanza EntityNotFoundException si no se encuentra hotel o habitación
    /// </summary>
    Task<Reserva> CreateAsync(Reserva reserva, string transactionId);

    /// <summary>
    /// BR-06: Cancelación lógica (no elimina registros)
    /// 
    /// Lanza EntityNotFoundException si no se encuentra la reserva
    /// Lanza BusinessRuleException si la reserva ya está cancelada
    /// </summary>
    Task CancelarAsync(int reservaId, string transactionId);
}
