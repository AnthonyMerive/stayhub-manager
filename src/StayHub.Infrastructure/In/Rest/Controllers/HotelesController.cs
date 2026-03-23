using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StayHub.Application.Ports.In.Services;
using StayHub.Infrastructure.In.Rest.Dtos;
using StayHub.Infrastructure.In.Rest.Extensions;
using StayHub.Infrastructure.In.Rest.Mappers;

namespace StayHub.Infrastructure.In.Rest.Controllers;

/// <summary>
/// Controller para gestión de hoteles
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class HotelesController(IHotelService hotelService) : ControllerBase
{
    /// <summary>
    /// Obtiene todos los hoteles con paginación
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResponseDto<PaginatedResult<HotelDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaginated(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        var result = await hotelService.GetPaginatedAsync(pageNumber, pageSize, transactionId, searchTerm);

        var paginatedResult = new PaginatedResult<HotelDto>
        {
            Items = HotelMapper.ToDtoList(result.Items),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages,
            TotalRecords = result.TotalRecords
        };

        return HttpResponseMapper.Ok(paginatedResult, transactionId);
    }

    /// <summary>
    /// Obtiene un hotel por su ID
    /// </summary>
    [HttpGet("{hotelId:int}")]
    [ProducesResponseType(typeof(ResponseDto<HotelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int hotelId)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        var hotel = await hotelService.GetByIdAsync(hotelId, transactionId);

        if (hotel == null)
        {
            return HttpResponseMapper.NotFound($"Hotel con ID {hotelId} no encontrado", transactionId);
        }

        return HttpResponseMapper.Ok(HotelMapper.ToDto(hotel), transactionId);
    }

    /// <summary>
    /// Crea un nuevo hotel
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ResponseDto<HotelDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateHotelRequest request)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        if (!ModelState.IsValid)
        {
            return HttpResponseMapper.BadRequest("Datos de entrada inválidos", transactionId);
        }

        var hotel = HotelMapper.ToEntity(request);
        var result = await hotelService.CreateAsync(hotel, transactionId);

        var dto = HotelMapper.ToDto(result);
        return HttpResponseMapper.Created(dto, transactionId);
    }

    /// <summary>
    /// Actualiza un hotel existente
    /// </summary>
    [HttpPut("{hotelId:int}")]
    [ProducesResponseType(typeof(ResponseDto<HotelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int hotelId, [FromBody] UpdateHotelRequest request)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        if (hotelId != request.HotelId)
        {
            return HttpResponseMapper.BadRequest("El ID del hotel no coincide", transactionId);
        }

        if (!ModelState.IsValid)
        {
            return HttpResponseMapper.BadRequest("Datos de entrada inválidos", transactionId);
        }

        var hotel = HotelMapper.ToEntity(request);
        var result = await hotelService.UpdateAsync(hotel, transactionId);

        return HttpResponseMapper.Ok(HotelMapper.ToDto(result), transactionId);
    }

    /// <summary>
    /// Activa o desactiva un hotel
    /// </summary>
    [HttpPatch("{hotelId:int}/estado")]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetEstado(int hotelId, [FromBody] SetEstadoRequest request)
    {
        var transactionId = HttpContext.GetTransactionId() ?? Guid.NewGuid().ToString();

        await hotelService.SetEstadoAsync(hotelId, request.Activo, transactionId);

        return HttpResponseMapper.Ok(transactionId);
    }
}
