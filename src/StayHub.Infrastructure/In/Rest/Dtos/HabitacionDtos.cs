using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace StayHub.Infrastructure.In.Rest.Dtos;

/// <summary>
/// DTO para respuesta de habitación
/// </summary>
[ExcludeFromCodeCoverage]
public class HabitacionDto
{
    public int HabitacionId { get; set; }
    public int HotelId { get; set; }
    public string NumeroHabitacion { get; set; } = string.Empty;
    public string TipoHabitacion { get; set; } = string.Empty;
    public int Capacidad { get; set; }
    public decimal TarifaNoche { get; set; }
    public string Estado { get; set; } = string.Empty;
}

/// <summary>
/// DTO para crear una habitación
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateHabitacionRequest
{
    [Required(ErrorMessage = "El ID del hotel es requerido")]
    public int HotelId { get; set; }

    [Required(ErrorMessage = "El número de habitación es requerido")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "El número debe tener entre 1 y 10 caracteres")]
    public string NumeroHabitacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de habitación es requerido")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El tipo debe tener entre 2 y 50 caracteres")]
    public string TipoHabitacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La capacidad es requerida")]
    [Range(1, 20, ErrorMessage = "La capacidad debe estar entre 1 y 20 personas")]
    public int Capacidad { get; set; }

    [Required(ErrorMessage = "La tarifa por noche es requerida")]
    [Range(0.01, 100000, ErrorMessage = "La tarifa debe ser mayor a 0")]
    public decimal TarifaNoche { get; set; }
}

/// <summary>
/// DTO para actualizar una habitación
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateHabitacionRequest
{
    [Required(ErrorMessage = "El ID de la habitación es requerido")]
    public int HabitacionId { get; set; }

    [Required(ErrorMessage = "El número de habitación es requerido")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "El número debe tener entre 1 y 10 caracteres")]
    public string NumeroHabitacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de habitación es requerido")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El tipo debe tener entre 2 y 50 caracteres")]
    public string TipoHabitacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La capacidad es requerida")]
    [Range(1, 20, ErrorMessage = "La capacidad debe estar entre 1 y 20 personas")]
    public int Capacidad { get; set; }

    [Required(ErrorMessage = "La tarifa por noche es requerida")]
    [Range(0.01, 100000, ErrorMessage = "La tarifa debe ser mayor a 0")]
    public decimal TarifaNoche { get; set; }
}

/// <summary>
/// DTO para consulta de disponibilidad
/// </summary>
[ExcludeFromCodeCoverage]
public class DisponibilidadRequest
{
    [Required(ErrorMessage = "El ID del hotel es requerido")]
    public int HotelId { get; set; }

    [Required(ErrorMessage = "La fecha de entrada es requerida")]
    public DateTime FechaEntrada { get; set; }

    [Required(ErrorMessage = "La fecha de salida es requerida")]
    public DateTime FechaSalida { get; set; }

    [Required(ErrorMessage = "La cantidad de huéspedes es requerida")]
    [Range(1, 20, ErrorMessage = "La cantidad de huéspedes debe estar entre 1 y 20")]
    public int CantidadHuespedes { get; set; }
}
