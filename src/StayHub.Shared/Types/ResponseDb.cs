using System.Diagnostics.CodeAnalysis;

namespace StayHub.Shared.Types;

/// <summary>
/// Respuesta estándar de operaciones de base de datos
/// </summary>
[ExcludeFromCodeCoverage]
public class ResponseDb
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public object? Data { get; set; }
}

/// <summary>
/// Respuesta genérica de operaciones de base de datos
/// </summary>
[ExcludeFromCodeCoverage]
public class ResponseDb<T> : ResponseDb
{
    public new T? Data { get; set; }
}
