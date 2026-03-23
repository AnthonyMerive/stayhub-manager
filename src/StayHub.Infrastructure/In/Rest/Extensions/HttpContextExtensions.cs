using Microsoft.AspNetCore.Http;
using StayHub.Infrastructure.In.Rest.Middlewares;

namespace StayHub.Infrastructure.In.Rest.Extensions;

/// <summary>
/// Extensiones para HttpContext
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Obtiene el transaction ID del contexto HTTP actual
    /// </summary>
    /// <param name="context">El contexto HTTP</param>
    /// <returns>El transaction ID si existe, null en caso contrario</returns>
    public static string? GetTransactionId(this HttpContext context)
    {
        return context.Items.TryGetValue(GlobalExceptionHandlingMiddleware.TransactionIdKey, out var transactionId) 
            ? transactionId?.ToString() 
            : null;
    }
}