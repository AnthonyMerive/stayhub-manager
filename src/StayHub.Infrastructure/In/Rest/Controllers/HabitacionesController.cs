using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StayHub.Application.Ports.In.Services;
using StayHub.Infrastructure.In.Rest.Dtos;
using StayHub.Infrastructure.In.Rest.Extensions;
using StayHub.Infrastructure.In.Rest.Mappers;

namespace StayHub.Infrastructure.In.Rest.Controllers;

/// <summary>
/// Controller para gestión de habitaciones
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class HabitacionesController(IHabitacionService habitacionService) : ControllerBase
{
    /// <summary>
    /// Obtiene todas las habitaciones con paginación
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResponseDto<PaginatedResult<HabitacionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaginated(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? hotelId = null)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        var result = await habitacionService.GetPaginatedAsync(pageNumber, pageSize, transactionId, hotelId);

        var paginatedResult = new PaginatedResult<HabitacionDto>
        {
            Items = HabitacionMapper.ToDtoList(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages,
            TotalRecords = result.TotalRecords
        };

        return HttpResponseMapper.Ok(paginatedResult, transactionId);
    }

    /// <summary>
    /// Obtiene una habitación por su ID
    /// </summary>
    [HttpGet("{habitacionId:int}")]
    [ProducesResponseType(typeof(ResponseDto<HabitacionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int habitacionId)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        var habitacion = await habitacionService.GetByIdAsync(habitacionId, transactionId);

        if (habitacion == null)
        {
            return HttpResponseMapper.NotFound($"Habitación con ID {habitacionId} no encontrada", transactionId);
        }

        return HttpResponseMapper.Ok(HabitacionMapper.ToDto(habitacion), transactionId);
    }

    /// <summary>
    /// Obtiene las habitaciones de un hotel
    /// </summary>
    [HttpGet("hotel/{hotelId:int}")]
    [ProducesResponseType(typeof(ResponseDto<List<HabitacionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByHotelId(int hotelId)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        var habitaciones = await habitacionService.GetByHotelIdAsync(hotelId, transactionId);
        return HttpResponseMapper.Ok(HabitacionMapper.ToDtoList(habitaciones), transactionId);
    }

    /// <summary>
    /// Consulta de disponibilidad de habitaciones
    /// BR-04: Solo retorna habitaciones activas con capacidad suficiente y sin overbooking
    /// </summary>
    [HttpPost("disponibilidad")]
    [ProducesResponseType(typeof(ResponseDto<List<HabitacionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDisponibles([FromBody] DisponibilidadRequest request)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        if (!ModelState.IsValid)
        {
            return HttpResponseMapper.BadRequest("Datos de entrada inválidos", transactionId);
        }

        // BR-01: Validar que fecha de salida > fecha de entrada
        if (request.FechaSalida <= request.FechaEntrada)
        {
            return HttpResponseMapper.BadRequest(
                "La fecha de salida debe ser posterior a la fecha de entrada", transactionId);
        }

        var habitaciones = await habitacionService.GetDisponiblesAsync(
            request.HotelId, request.FechaEntrada, request.FechaSalida, request.CantidadHuespedes, transactionId);

        return HttpResponseMapper.Ok(HabitacionMapper.ToDtoList(habitaciones), transactionId);
    }

    /// <summary>
    /// Crea una nueva habitación
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ResponseDto<HabitacionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateHabitacionRequest request)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        if (!ModelState.IsValid)
        {
            return HttpResponseMapper.BadRequest("Datos de entrada inválidos", transactionId);
        }

        var habitacion = HabitacionMapper.ToEntity(request);
        var result = await habitacionService.CreateAsync(habitacion, transactionId);

        var dto = HabitacionMapper.ToDto(result);
        return HttpResponseMapper.Created(dto, transactionId);
    }

    /// <summary>
    /// Actualiza una habitación existente
    /// </summary>
    [HttpPut("{habitacionId:int}")]
    [ProducesResponseType(typeof(ResponseDto<HabitacionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int habitacionId, [FromBody] UpdateHabitacionRequest request)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        if (habitacionId != request.HabitacionId)
        {
            return HttpResponseMapper.BadRequest("El ID de la habitación no coincide", transactionId);
        }

        if (!ModelState.IsValid)
        {
            return HttpResponseMapper.BadRequest("Datos de entrada inválidos", transactionId);
        }

        var habitacion = HabitacionMapper.ToEntity(request);
        var result = await habitacionService.UpdateAsync(habitacion, transactionId);

        return HttpResponseMapper.Ok(HabitacionMapper.ToDto(result), transactionId);
    }

    /// <summary>
    /// Activa o desactiva una habitación
    /// </summary>
    [HttpPatch("{habitacionId:int}/estado")]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetEstado(int habitacionId, [FromBody] SetEstadoRequest request)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        await habitacionService.SetEstadoAsync(habitacionId, request.Activo, transactionId);

        return HttpResponseMapper.Ok(transactionId);
    }
}
