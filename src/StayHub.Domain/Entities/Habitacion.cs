using StayHub.Domain.Enums;

namespace StayHub.Domain.Entities;

/// <summary>
/// Entidad que representa una habitación de hotel
/// </summary>
public class Habitacion
{
    public int HabitacionId { get; set; }
    public int HotelId { get; set; }
    public string NumeroHabitacion { get; set; } = string.Empty;
    public string TipoHabitacion { get; set; } = string.Empty;
    public int Capacidad { get; set; }
    public decimal TarifaNoche { get; set; }
    public Estado Estado { get; set; } = Estado.Activo;

    // Navegación
    public virtual Hotel? Hotel { get; set; }
    public virtual ICollection<Reserva> Reservas { get; set; } = [];
}
