using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace StayHub.Infrastructure.In.Rest.Dtos;

/// <summary>
/// DTO para respuesta de reserva
/// </summary>
[ExcludeFromCodeCoverage]
public class ReservaDto
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
    public string EstadoReserva { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public int NochesEstadia { get; set; }
}

/// <summary>
/// DTO para crear una reserva
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateReservaRequest
{
    [Required(ErrorMessage = "El ID del hotel es requerido")]
    public int HotelId { get; set; }

    [Required(ErrorMessage = "El ID de la habitación es requerido")]
    public int HabitacionId { get; set; }

    [Required(ErrorMessage = "El nombre del huésped es requerido")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    public string HuespedNombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El documento del huésped es requerido")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "El documento debe tener entre 5 y 20 caracteres")]
    public string HuespedDocumento { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de entrada es requerida")]
    public DateTime FechaEntrada { get; set; }

    [Required(ErrorMessage = "La fecha de salida es requerida")]
    public DateTime FechaSalida { get; set; }

    [Required(ErrorMessage = "La cantidad de huéspedes es requerida")]
    [Range(1, 20, ErrorMessage = "La cantidad de huéspedes debe estar entre 1 y 20")]
    public int CantidadHuespedes { get; set; }
}
