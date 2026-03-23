namespace StayHub.Application.Rules.Constants;

/// <summary>
/// Plantillas de mensajes de error utilizados en el servicio de hoteles
/// </summary>
public static class HotelErrorMessageTemplates
{
    public const string ErrorObtenerHotel = "Error al obtener hotel: {0}";
    public const string ErrorObtenerHoteles = "Error al obtener hoteles: {0}";
    public const string ErrorObtenerHotelesPaginados = "Error al obtener hoteles paginados: {0}";
    public const string ErrorCrearHotel = "Error al crear hotel: {0}";
    public const string ErrorActualizarHotel = "Error al actualizar hotel: {0}";
    public const string ErrorVerificarHotelExistente = "Error al verificar hotel existente: {0}";
    public const string ErrorCambiarEstadoHotel = "Error al cambiar estado del hotel: {0}";
}