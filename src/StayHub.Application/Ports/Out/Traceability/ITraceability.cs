namespace StayHub.Application.Ports.Out.Traceability;

/// <summary>
/// Puerto de salida para el sistema de trazabilidad
/// </summary>
public interface ITraceability
{
    /// <summary>
    /// Registra una entrada de trace de información
    /// </summary>
    Task TraceInAsync(string transactionId, string operation, string message, Dictionary<string, object>? additionalProperties = null);

    /// <summary>
    /// Registra una entrada de trace de advertencia
    /// </summary>
    Task TraceOutAsync(string transactionId, string operation, string message, Dictionary<string, object>? additionalProperties = null);

    /// <summary>
    /// Registra una entrada de trace de error
    /// </summary>
    Task TraceErrorAsync(string transactionId, string operation, Exception exception, Dictionary<string, object>? additionalProperties = null);
}