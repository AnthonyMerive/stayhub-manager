namespace StayHub.Domain.Constants.Reserva;

/// <summary>
/// Mensajes de error utilizados en el servicio de reservas
/// </summary>
public static class ReservaErrorMessages
{
    public const string InvalidReservaId = "El ID de la reserva debe ser un valor positivo.";
    public const string InvalidHotelId = "El ID del hotel debe ser un valor positivo.";
    public const string InvalidHabitacionId = "El ID de la habitación debe ser un valor positivo.";
    public const string HabitacionInactiva = "La habitación seleccionada no está disponible.";
    public const string ReservaYaCancelada = "La reserva ya se encuentra cancelada.";
    public const string OverbookingDetected = "La habitación ya tiene reservas activas para el período solicitado.";
}