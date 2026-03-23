using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StayHub.Application.Ports.In.Services;
using StayHub.Infrastructure.In.Rest.Dtos;
using StayHub.Infrastructure.In.Rest.Extensions;
using StayHub.Infrastructure.In.Rest.Mappers;

namespace StayHub.Infrastructure.In.Rest.Controllers;

/// <summary>
/// Controller para gestión de reservas
/// Implementa las reglas de negocio BR-01 a BR-06
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ReservasController(IReservaService reservaService) : ControllerBase
{
    /// <summary>
    /// Obtiene todas las reservas con paginación
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResponseDto<PaginatedResult<ReservaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaginated(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? hotelId = null,
        [FromQuery] int? habitacionId = null)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        var result = await reservaService.GetPaginatedAsync(pageNumber, pageSize, transactionId, hotelId, habitacionId);

        var paginatedResult = new PaginatedResult<ReservaDto>
        {
            Items = ReservaMapper.ToDtoList(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages,
            TotalRecords = result.TotalRecords
        };

        return HttpResponseMapper.Ok(paginatedResult, transactionId);
    }

    /// <summary>
    /// Obtiene una reserva por su ID
    /// </summary>
    [HttpGet("{reservaId:int}")]
    [ProducesResponseType(typeof(ResponseDto<ReservaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int reservaId)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        var reserva = await reservaService.GetByIdAsync(reservaId, transactionId);

        if (reserva == null)
        {
            return HttpResponseMapper.NotFound($"Reserva con ID {reservaId} no encontrada", transactionId);
        }

        return HttpResponseMapper.Ok(ReservaMapper.ToDto(reserva), transactionId);
    }

    /// <summary>
    /// Obtiene las reservas de un hotel
    /// </summary>
    [HttpGet("hotel/{hotelId:int}")]
    [ProducesResponseType(typeof(ResponseDto<List<ReservaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByHotelId(int hotelId)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        var reservas = await reservaService.GetByHotelIdAsync(hotelId, transactionId);
        return HttpResponseMapper.Ok(ReservaMapper.ToDtoList(reservas), transactionId);
    }

    /// <summary>
    /// Crea una nueva reserva aplicando todas las reglas de negocio:
    /// - BR-01: Validación de fechas (salida > entrada)
    /// - BR-02: Prevención de overbooking
    /// - BR-03: Validación de capacidad
    /// - BR-04: Solo habitaciones activas
    /// - BR-05: Cálculo automático del total
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ResponseDto<ReservaDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateReservaRequest request)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        if (!ModelState.IsValid)
        {
            return HttpResponseMapper.BadRequest("Datos de entrada inválidos", transactionId);
        }

        var reserva = ReservaMapper.ToEntity(request);
        var result = await reservaService.CreateAsync(reserva, transactionId);

        var dto = ReservaMapper.ToDto(result);
        return HttpResponseMapper.Created(dto, transactionId);
    }

    /// <summary>
    /// Cancela una reserva (BR-06: cancelación lógica, no elimina registros)
    /// </summary>
    [HttpPost("{reservaId:int}/cancelar")]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancelar(int reservaId)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        await reservaService.CancelarAsync(reservaId, transactionId);

        return HttpResponseMapper.Ok(transactionId);
    }
}
