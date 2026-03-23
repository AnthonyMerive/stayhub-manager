using Microsoft.Extensions.Logging;
using StayHub.Application.Ports.Out.Traceability;
using StayHub.Domain.Exceptions;
using System.Text.Json;

namespace StayHub.Infrastructure.Out.Traceability.Logging;

/// <summary>
/// Adaptador de trazabilidad que implementa el puerto de salida usando ILogger de .NET
/// </summary>
public class TraceabilityAdapter(ILogger<TraceabilityAdapter> logger) : ITraceability
{
    private readonly ILogger<TraceabilityAdapter> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public Task TraceInAsync(string transactionId, string operation, string message, Dictionary<string, object>? additionalProperties = null)
    {
        return TraceAsync(transactionId, "IN", operation, message, additionalProperties, LogLevel.Information);
    }

    public Task TraceOutAsync(string transactionId, string operation, string message, Dictionary<string, object>? additionalProperties = null)
    {
        return TraceAsync(transactionId, "OUT", operation, message, additionalProperties, LogLevel.Information);
    }

    public Task TraceErrorAsync(string transactionId, string operation, Exception exception, Dictionary<string, object>? additionalProperties = null)
    {
        if (exception is BusinessException)
        {
            return TraceAsync(transactionId, "BUSINESS_ERROR", operation, exception.Message, additionalProperties, LogLevel.Warning);
        }

        return TraceAsync(transactionId, "TECHNICAL_ERROR", operation, exception.Message, additionalProperties, LogLevel.Error);
    }

    private Task TraceAsync(string transactionId, string type, string operation, string message, Dictionary<string, object>? additionalProperties, LogLevel logLevel)
    {
        _logger.Log(logLevel, "[{Type}] [{Operation}] [{TransactionId}] [{Message}] : {Details}",
            type,
            operation,
            transactionId,
            message,
            CreateDetails(additionalProperties));

        return Task.CompletedTask;
    }

    private static string CreateDetails(Dictionary<string, object>? additionalProperties)
    {
        return JsonSerializer.Serialize(additionalProperties ?? [], _jsonSerializerOptions);
    }
}