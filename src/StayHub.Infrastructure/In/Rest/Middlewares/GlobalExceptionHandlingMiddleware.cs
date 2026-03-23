using Microsoft.AspNetCore.Http;
using StayHub.Domain.Exceptions;
using StayHub.Infrastructure.In.Rest.Dtos;
using System.Net;
using System.Text.Json;

namespace StayHub.Infrastructure.In.Rest.Middlewares;

/// <summary>
/// Middleware para manejo global de excepciones
/// Convierte excepciones de negocio en respuestas HTTP apropiadas y las registra en la trazabilidad
/// </summary>
public class GlobalExceptionHandlingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    public const string TransactionIdKey = "TransactionId";

    public async Task InvokeAsync(HttpContext context)
    {
        var transactionId = Guid.NewGuid().ToString();

        // Almacenar el transactionId en el contexto HTTP para acceso global
        context.Items[TransactionIdKey] = transactionId;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, transactionId);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, string transactionId)
    {
        context.Response.ContentType = "application/json";

        var statusCode = GetStatusCode(exception);
        var errorMessage = GetErrorMessage(exception);

        var response = ResponseDto.Error(errorMessage, transactionId, (int)statusCode);

        context.Response.StatusCode = (int)statusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private static HttpStatusCode GetStatusCode(Exception exception) =>
        exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            BusinessException ex => (HttpStatusCode)ex.HttpStatusCode,
            ArgumentException => HttpStatusCode.BadRequest,
            InvalidOperationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

    private static string GetErrorMessage(Exception exception) =>
        exception switch
        {
            NotFoundException ex => ex.Message,
            BusinessException ex => ex.Message,
            ArgumentException ex => ex.Message,
            InvalidOperationException ex => ex.Message,
            _ => "Ha ocurrido un error interno del servidor."
        };
}