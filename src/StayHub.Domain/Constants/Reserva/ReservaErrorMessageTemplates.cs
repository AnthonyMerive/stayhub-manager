namespace StayHub.Domain.Constants.Reserva;

/// <summary>
/// Plantillas de mensajes de error utilizados en el servicio de reservas
/// </summary>
public static class ReservaErrorMessageTemplates
{
    public const string ErrorObtenerReserva = "Error al obtener reserva: {0}";
    public const string ErrorObtenerReservasHotel = "Error al obtener reservas del hotel: {0}";
    public const string ErrorObtenerReservasPaginadas = "Error al obtener reservas paginadas: {0}";
    public const string ErrorVerificarHabitacion = "Error al verificar habitación: {0}";
    public const string ErrorCrearReserva = "Error al crear reserva: {0}";
    public const string ErrorVerificarReservaExistente = "Error al verificar reserva existente: {0}";
    public const string ErrorCancelarReserva = "Error al cancelar reserva: {0}";
    public const string ErrorValidarOverbooking = "Error al validar overbooking: {0}";
    public const string CapacidadExcedida = "El número de huéspedes ({0}) excede la capacidad de la habitación ({1}).";
}