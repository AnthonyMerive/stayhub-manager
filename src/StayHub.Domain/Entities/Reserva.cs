using StayHub.Domain.Enums;
using StayHub.Domain.Exceptions;

namespace StayHub.Domain.Entities;

/// <summary>
/// Entidad que representa una reserva de habitación
/// </summary>
public class Reserva
{
    public int ReservaId { get; set; }
    public int HotelId { get; set; }
    public int HabitacionId { get; set; }
    public string HuespedNombre { get; set; } = string.Empty;
    public string HuespedDocumento { get; set; } = string.Empty;
    public DateTime FechaEntrada { get; set; }
    public DateTime FechaSalida { get; set; }
    public int CantidadHuespedes { get; set; }
    public decimal ValorNoche { get; set; }
    public decimal TotalReserva { get; set; }
    public EstadoReserva EstadoReserva { get; set; } = EstadoReserva.Activa;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual Hotel? Hotel { get; set; }
    public virtual Habitacion? Habitacion { get; set; }

    /// <summary>
    /// BR-01: Valida que la fecha de salida sea mayor a la fecha de entrada
    /// </summary>
    public void ValidarFechas()
    {
        if (FechaSalida <= FechaEntrada)
        {
            throw new BusinessException("BR-01", 
                "La fecha de salida debe ser estrictamente mayor a la fecha de entrada.");
        }

        // Validación adicional: no se pueden hacer reservas en fechas pasadas
        if (FechaEntrada.Date < DateTime.UtcNow.Date)
        {
            throw new BusinessException("BR-01-PAST", 
                "No se pueden crear reservas con fecha de entrada en el pasado.");
        }

        // Validación adicional: estadía mínima de 1 noche
        if (ObtenerNochesEstadia() < 1)
        {
            throw new BusinessException("BR-01-MIN", 
                "La reserva debe tener al menos una noche de estadía.");
        }

        // Validación adicional: máximo 90 días de estadía
        if (ObtenerNochesEstadia() > 90)
        {
            throw new BusinessException("BR-01-MAX", 
                "La reserva no puede exceder los 90 días de estadía.");
        }
    }

    /// <summary>
    /// BR-03: Valida que la cantidad de huéspedes no exceda la capacidad de la habitación
    /// </summary>
    public void ValidarCapacidad(int capacidadHabitacion)
    {
        if (CantidadHuespedes <= 0)
        {
            throw new BusinessException("BR-03-MIN", 
                "La cantidad de huéspedes debe ser mayor a cero.");
        }

        if (CantidadHuespedes > capacidadHabitacion)
        {
            throw new BusinessException("BR-03", 
                $"La cantidad de huéspedes ({CantidadHuespedes}) excede la capacidad máxima de la habitación ({capacidadHabitacion}).");
        }
    }

    /// <summary>
    /// Valida los datos básicos del huésped
    /// </summary>
    public void ValidarDatosHuesped()
    {
        if (string.IsNullOrWhiteSpace(HuespedNombre))
        {
            throw new BusinessException("BR-GUEST-NAME", 
                "El nombre del huésped es obligatorio.");
        }

        if (HuespedNombre.Length < 2)
        {
            throw new BusinessException("BR-GUEST-NAME-MIN", 
                "El nombre del huésped debe tener al menos 2 caracteres.");
        }

        if (HuespedNombre.Length > 100)
        {
            throw new BusinessException("BR-GUEST-NAME-MAX", 
                "El nombre del huésped no puede exceder los 100 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(HuespedDocumento))
        {
            throw new BusinessException("BR-GUEST-DOC", 
                "El documento del huésped es obligatorio.");
        }

        if (HuespedDocumento.Length < 5)
        {
            throw new BusinessException("BR-GUEST-DOC-MIN", 
                "El documento del huésped debe tener al menos 5 caracteres.");
        }
    }

    /// <summary>
    /// BR-05: Calcula el total de la reserva (Noches × Tarifa)
    /// </summary>
    public void CalcularTotal()
    {
        var noches = (FechaSalida - FechaEntrada).Days;
        TotalReserva = noches * ValorNoche;
    }

    /// <summary>
    /// Obtiene el número de noches de la estadía
    /// </summary>
    public int ObtenerNochesEstadia()
    {
        return (FechaSalida - FechaEntrada).Days;
    }
}
