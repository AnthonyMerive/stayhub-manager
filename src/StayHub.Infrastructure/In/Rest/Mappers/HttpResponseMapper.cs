using Microsoft.AspNetCore.Mvc;
using StayHub.Infrastructure.In.Rest.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace StayHub.Infrastructure.In.Rest.Mappers;

/// <summary>
/// Adaptador HTTP que maneja la conversión automática a ResponseDto
/// </summary>
[ExcludeFromCodeCoverage]
public static class HttpResponseMapper
{
    /// <summary>
    /// Crea una respuesta HTTP exitosa con datos
    /// </summary>
    public static ObjectResult Ok<T>(T data, string? transactionId = null)
    {
        var response = ResponseDto<T>.Success(data, transactionId, 200);
        return new OkObjectResult(response);
    }

    /// <summary>
    /// Crea una respuesta HTTP exitosa sin datos
    /// </summary>
    public static ObjectResult Ok(string? transactionId = null)
    {
        var response = ResponseDto.Success(transactionId, 200);
        return new OkObjectResult(response);
    }

    /// <summary>
    /// Crea una respuesta HTTP de recurso creado
    /// </summary>
    public static ObjectResult Created<T>(T data, string? transactionId = null)
    {
        var response = ResponseDto<T>.Success(data, transactionId, 201);
        return new ObjectResult(response) { StatusCode = 201 };
    }

    /// <summary>
    /// Crea una respuesta HTTP de recurso creado sin datos
    /// </summary>
    public static ObjectResult Created(string? transactionId = null)
    {
        var response = ResponseDto.Success(transactionId, 201);
        return new ObjectResult(response) { StatusCode = 201 };
    }

    /// <summary>
    /// Crea una respuesta HTTP sin contenido (204)
    /// </summary>
    public static ObjectResult NoContent(string? transactionId = null)
    {
        var response = ResponseDto.Success(transactionId, 204);
        return new ObjectResult(response) { StatusCode = 204 };
    }

    /// <summary>
    /// Crea una respuesta HTTP de solicitud incorrecta
    /// </summary>
    public static ObjectResult BadRequest(string errorMessage, string? transactionId = null)
    {
        var response = ResponseDto.Error(errorMessage, transactionId, 400);
        return new BadRequestObjectResult(response);
    }

    /// <summary>
    /// Crea una respuesta HTTP de no encontrado
    /// </summary>
    public static ObjectResult NotFound(string errorMessage, string? transactionId = null)
    {
        var response = ResponseDto.Error(errorMessage, transactionId, 404);
        return new NotFoundObjectResult(response);
    }

    /// <summary>
    /// Crea una respuesta HTTP de conflicto
    /// </summary>
    public static ObjectResult Conflict(string errorMessage, string? transactionId = null)
    {
        var response = ResponseDto.Error(errorMessage, transactionId, 409);
        return new ObjectResult(response) { StatusCode = 409 };
    }

    /// <summary>
    /// Crea una respuesta HTTP de error interno del servidor
    /// </summary>
    public static ObjectResult InternalServerError(string errorMessage, string? transactionId = null)
    {
        var response = ResponseDto.Error(errorMessage, transactionId, 500);
        return new ObjectResult(response) { StatusCode = 500 };
    }
}