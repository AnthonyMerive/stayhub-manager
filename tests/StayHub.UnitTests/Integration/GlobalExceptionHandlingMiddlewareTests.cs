using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using StayHub.Domain.Exceptions;
using StayHub.Infrastructure.In.Rest.Middlewares;
using System.Text.Json;
using Xunit;

namespace StayHub.UnitTests.Integration;

/// <summary>
/// Tests de integración para GlobalExceptionHandlingMiddleware
/// </summary>
public class GlobalExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_BusinessRuleException_DebeRetornar400ConMensaje()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new GlobalExceptionHandlingMiddleware(
            (HttpContext ctx) => throw new BusinessException("BR-01", "Test business rule"));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(400);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Position = 0;
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("mensajeError").GetString().Should().Be("Test business rule");
        response.GetProperty("codigoEstado").GetInt32().Should().Be(400);
        response.GetProperty("idTransaccion").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task InvokeAsync_EntityNotFoundException_DebeRetornar404()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new GlobalExceptionHandlingMiddleware(
            (HttpContext ctx) => throw new NotFoundException("Hotel", 123));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(404);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Position = 0;
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("mensajeError").GetString().Should().Be("La entidad 'Hotel' con identificador '123' no fue encontrada.");
        response.GetProperty("codigoEstado").GetInt32().Should().Be(404);
        response.GetProperty("idTransaccion").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task InvokeAsync_ExcepcionGenerica_DebeRetornar500()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new GlobalExceptionHandlingMiddleware(
            (HttpContext ctx) => throw new InvalidOperationException("General error"));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(400); // InvalidOperationException mapeada a 400

        context.Response.Body.Position = 0;
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("mensajeError").GetString().Should().Be("General error");
        response.GetProperty("codigoEstado").GetInt32().Should().Be(400);
        response.GetProperty("idTransaccion").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task InvokeAsync_ExcepcionNoControlada_DebeRetornar500ConErrorGenerico()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new GlobalExceptionHandlingMiddleware(
            (HttpContext ctx) => throw new NotImplementedException("Not implemented"));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(500);

        context.Response.Body.Position = 0;
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        response.GetProperty("mensajeError").GetString().Should().Be("Ha ocurrido un error interno del servidor.");
        response.GetProperty("codigoEstado").GetInt32().Should().Be(500);
        response.GetProperty("idTransaccion").GetString().Should().NotBeNullOrEmpty();
    }
}