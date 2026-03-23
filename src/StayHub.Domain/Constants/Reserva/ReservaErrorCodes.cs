namespace StayHub.Domain.Constants.Reserva;

/// <summary>
/// Códigos de error utilizados en el servicio de reservas
/// </summary>
public static class ReservaErrorCodes
{
    public const string InvalidReservaId = "INVALID_RESERVA_ID";
    public const string InvalidHotelId = "INVALID_HOTEL_ID";
    public const string InvalidHabitacionId = "INVALID_HABITACION_ID";
    public const string HabitacionInactiva = "HABITACION_INACTIVA";
    public const string CapacidadExcedida = "CAPACIDAD_EXCEDIDA";
    public const string ReservaYaCancelada = "RESERVA_YA_CANCELADA";
    public const string OverbookingDetected = "OVERBOOKING_DETECTED";
}