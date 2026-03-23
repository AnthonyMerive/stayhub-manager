using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace StayHub.Infrastructure.In.Rest.Dtos;

/// <summary>
/// DTO para respuesta de hotel
/// </summary>
[ExcludeFromCodeCoverage]
public class HotelDto
{
    public int HotelId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}

/// <summary>
/// DTO para crear un hotel
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateHotelRequest
{
    [Required(ErrorMessage = "El nombre del hotel es requerido")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ciudad es requerida")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "La ciudad debe tener entre 2 y 50 caracteres")]
    public string Ciudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es requerida")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "La dirección debe tener entre 5 y 200 caracteres")]
    public string Direccion { get; set; } = string.Empty;
}

/// <summary>
/// DTO para actualizar un hotel
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateHotelRequest
{
    [Required(ErrorMessage = "El ID del hotel es requerido")]
    public int HotelId { get; set; }

    [Required(ErrorMessage = "El nombre del hotel es requerido")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ciudad es requerida")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "La ciudad debe tener entre 2 y 50 caracteres")]
    public string Ciudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es requerida")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "La dirección debe tener entre 5 y 200 caracteres")]
    public string Direccion { get; set; } = string.Empty;
}

/// <summary>
/// DTO para cambiar estado del hotel
/// </summary>
[ExcludeFromCodeCoverage]
public class SetEstadoRequest
{
    [Required(ErrorMessage = "El estado es requerido")]
    public bool Activo { get; set; }
}
