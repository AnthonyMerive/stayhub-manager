using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace StayHub.Infrastructure.In.Rest.Dtos;

/// <summary>
/// Respuesta genérica estandarizada para todas las API
/// </summary>
[ExcludeFromCodeCoverage]
public class ResponseDto<T>
{
    [JsonPropertyName("idTransaccion")]
    public string? TransactionId { get; set; }

    [JsonPropertyName("mensajeError")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("codigoEstado")]
    public int StatusCode { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    /// <summary>
    /// Crea una respuesta exitosa
    /// </summary>
    public static ResponseDto<T> Success(T? data, string? transactionId = null, int statusCode = 200)
    {
        return new ResponseDto<T>
        {
            TransactionId = transactionId ?? Guid.NewGuid().ToString(),
            ErrorMessage = null,
            StatusCode = statusCode,
            Data = data
        };
    }

    /// <summary>
    /// Crea una respuesta de error
    /// </summary>
    public static ResponseDto<T> Error(string errorMessage, string? transactionId = null, int statusCode = 500)
    {
        return new ResponseDto<T>
        {
            TransactionId = transactionId ?? Guid.NewGuid().ToString(),
            ErrorMessage = errorMessage,
            StatusCode = statusCode,
            Data = default
        };
    }
}

/// <summary>
/// Respuesta genérica sin datos
/// </summary>
[ExcludeFromCodeCoverage]
public class ResponseDto : ResponseDto<object>
{
    /// <summary>
    /// Crea una respuesta exitosa sin datos
    /// </summary>
    public static ResponseDto Success(string? transactionId = null, int statusCode = 200)
    {
        return new ResponseDto
        {
            TransactionId = transactionId ?? Guid.NewGuid().ToString(),
            ErrorMessage = null,
            StatusCode = statusCode,
            Data = null
        };
    }

    /// <summary>
    /// Crea una respuesta de error sin datos
    /// </summary>
    public new static ResponseDto Error(string errorMessage, string? transactionId = null, int statusCode = 500)
    {
        return new ResponseDto
        {
            TransactionId = transactionId ?? Guid.NewGuid().ToString(),
            ErrorMessage = errorMessage,
            StatusCode = statusCode,
            Data = null
        };
    }
}