using StayHub.Domain.Enums;

namespace StayHub.Domain.Entities;

/// <summary>
/// Entidad que representa un hotel en el sistema
/// </summary>
public class Hotel
{
    public int HotelId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public Estado Estado { get; set; } = Estado.Activo;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual ICollection<Habitacion> Habitaciones { get; set; } = [];
    public virtual ICollection<Reserva> Reservas { get; set; } = [];
}
