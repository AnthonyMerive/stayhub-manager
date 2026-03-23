using FluentAssertions;
using StayHub.Domain.Entities;
using StayHub.Domain.Enums;
using StayHub.Domain.Exceptions;
using Xunit;

namespace StayHub.UnitTests.Domain;

/// <summary>
/// Tests unitarios para la entidad Reserva
/// Valida las reglas de negocio BR-01, BR-03 y BR-05
/// </summary>
public class ReservaTests
{
    /// <summary>
    /// BR-01: La fecha de salida debe ser estrictamente mayor a la fecha de entrada
    /// </summary>
    [Fact]
    public void ValidarFechas_FechaSalidaMayorAEntrada_NoLanzaExcepcion()
    {
        // Arrange
        var reserva = new Reserva
        {
            FechaEntrada = DateTime.Today.AddDays(1),
            FechaSalida = DateTime.Today.AddDays(3)
        };

        // Act
        var action = () => reserva.ValidarFechas();

        // Assert
        action.Should().NotThrow();
    }

    /// <summary>
    /// BR-01: Debe lanzar excepción si fecha de salida es igual a fecha de entrada
    /// </summary>
    [Fact]
    public void ValidarFechas_FechasIguales_LanzaBusinessRuleException()
    {
        // Arrange
        var fecha = DateTime.Today;
        var reserva = new Reserva
        {
            FechaEntrada = fecha,
            FechaSalida = fecha
        };

        // Act
        var action = () => reserva.ValidarFechas();

        // Assert
        action.Should().Throw<BusinessException>()
            .WithMessage("*fecha de salida debe ser estrictamente mayor*")
            .Where(e => e.RuleCode == "BR-01");
    }

    /// <summary>
    /// BR-01: Debe lanzar excepción si fecha de salida es menor a fecha de entrada
    /// </summary>
    [Fact]
    public void ValidarFechas_FechaSalidaMenorQueEntrada_LanzaBusinessRuleException()
    {
        // Arrange
        var reserva = new Reserva
        {
            FechaEntrada = DateTime.Today.AddDays(3),
            FechaSalida = DateTime.Today
        };

        // Act
        var action = () => reserva.ValidarFechas();

        // Assert
        action.Should().Throw<BusinessException>()
            .Where(e => e.RuleCode == "BR-01");
    }

    /// <summary>
    /// BR-03: La cantidad de huéspedes no debe exceder la capacidad
    /// </summary>
    [Theory]
    [InlineData(2, 4)] // 2 huéspedes, capacidad 4 - OK
    [InlineData(4, 4)] // 4 huéspedes, capacidad 4 - OK (igual a capacidad)
    [InlineData(1, 2)] // 1 huésped, capacidad 2 - OK
    public void ValidarCapacidad_HuespedesDentroDeCapacidad_NoLanzaExcepcion(int huespedes, int capacidad)
    {
        // Arrange
        var reserva = new Reserva { CantidadHuespedes = huespedes };

        // Act
        var action = () => reserva.ValidarCapacidad(capacidad);

        // Assert
        action.Should().NotThrow();
    }

    /// <summary>
    /// BR-03: Debe lanzar excepción si huéspedes exceden capacidad
    /// </summary>
    [Theory]
    [InlineData(5, 4)] // 5 huéspedes, capacidad 4 - ERROR
    [InlineData(3, 2)] // 3 huéspedes, capacidad 2 - ERROR
    public void ValidarCapacidad_HuespedesExcedenCapacidad_LanzaBusinessRuleException(int huespedes, int capacidad)
    {
        // Arrange
        var reserva = new Reserva { CantidadHuespedes = huespedes };

        // Act
        var action = () => reserva.ValidarCapacidad(capacidad);

        // Assert
        action.Should().Throw<BusinessException>()
            .WithMessage($"*{huespedes}*excede*{capacidad}*")
            .Where(e => e.RuleCode == "BR-03");
    }

    /// <summary>
    /// BR-05: El total debe ser (Noches × Tarifa)
    /// </summary>
    [Theory]
    [InlineData(1, 100000, 100000)]   // 1 noche × $100,000 = $100,000
    [InlineData(3, 150000, 450000)]   // 3 noches × $150,000 = $450,000
    [InlineData(7, 200000, 1400000)]  // 7 noches × $200,000 = $1,400,000
    public void CalcularTotal_DebeCalcularCorrectamente(int noches, decimal tarifaNoche, decimal totalEsperado)
    {
        // Arrange
        var reserva = new Reserva
        {
            FechaEntrada = DateTime.Today,
            FechaSalida = DateTime.Today.AddDays(noches),
            ValorNoche = tarifaNoche
        };

        // Act
        reserva.CalcularTotal();

        // Assert
        reserva.TotalReserva.Should().Be(totalEsperado);
    }

    /// <summary>
    /// Obtener noches de estadía
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(7)]
    [InlineData(30)]
    public void ObtenerNochesEstadia_DebeRetornarNochesCorrectas(int nochesEsperadas)
    {
        // Arrange
        var reserva = new Reserva
        {
            FechaEntrada = DateTime.Today,
            FechaSalida = DateTime.Today.AddDays(nochesEsperadas)
        };

        // Act
        var noches = reserva.ObtenerNochesEstadia();

        // Assert
        noches.Should().Be(nochesEsperadas);
    }

    /// <summary>
    /// BR-01-PAST: No se pueden hacer reservas con fecha de entrada en el pasado
    /// </summary>
    [Fact]
    public void ValidarFechas_FechaEntradaEnPasado_LanzaBusinessRuleException()
    {
        // Arrange
        var reserva = new Reserva
        {
            FechaEntrada = DateTime.UtcNow.AddDays(-1),
            FechaSalida = DateTime.UtcNow.AddDays(3)
        };

        // Act
        var action = () => reserva.ValidarFechas();

        // Assert
        action.Should().Throw<BusinessException>()
            .WithMessage("*fecha de entrada en el pasado*")
            .Where(e => e.RuleCode == "BR-01-PAST");
    }

    /// <summary>
    /// BR-01-MAX: La reserva no puede exceder los 90 días
    /// </summary>
    [Fact]
    public void ValidarFechas_EstadiaExcede90Dias_LanzaBusinessRuleException()
    {
        // Arrange
        var reserva = new Reserva
        {
            FechaEntrada = DateTime.UtcNow.AddDays(1),
            FechaSalida = DateTime.UtcNow.AddDays(92) // 91 noches de estadía
        };

        // Act
        var action = () => reserva.ValidarFechas();

        // Assert
        action.Should().Throw<BusinessException>()
            .WithMessage("*no puede exceder los 90 días*")
            .Where(e => e.RuleCode == "BR-01-MAX");
    }

    /// <summary>
    /// BR-03-MIN: La cantidad de huéspedes debe ser mayor a cero
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ValidarCapacidad_CantidadHuespedesCeroONegativa_LanzaBusinessRuleException(int huespedes)
    {
        // Arrange
        var reserva = new Reserva { CantidadHuespedes = huespedes };

        // Act
        var action = () => reserva.ValidarCapacidad(4);

        // Assert
        action.Should().Throw<BusinessException>()
            .WithMessage("*cantidad de huéspedes debe ser mayor a cero*")
            .Where(e => e.RuleCode == "BR-03-MIN");
    }

    /// <summary>
    /// BR-GUEST-NAME: El nombre del huésped es obligatorio
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ValidarDatosHuesped_NombreVacio_LanzaBusinessRuleException(string? nombre)
    {
        // Arrange
        var reserva = new Reserva 
        { 
            HuespedNombre = nombre ?? string.Empty,
            HuespedDocumento = "12345678"
        };

        // Act
        var action = () => reserva.ValidarDatosHuesped();

        // Assert
        action.Should().Throw<BusinessException>()
            .WithMessage("*nombre del huésped es obligatorio*")
            .Where(e => e.RuleCode == "BR-GUEST-NAME");
    }

    /// <summary>
    /// BR-GUEST-DOC: El documento del huésped es obligatorio
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("123")] // Muy corto
    public void ValidarDatosHuesped_DocumentoInvalido_LanzaBusinessRuleException(string? documento)
    {
        // Arrange
        var reserva = new Reserva 
        { 
            HuespedNombre = "Juan Pérez",
            HuespedDocumento = documento ?? string.Empty
        };

        // Act
        var action = () => reserva.ValidarDatosHuesped();

        // Assert
        action.Should().Throw<BusinessException>()
            .Where(e => e.RuleCode.StartsWith("BR-GUEST-DOC"));
    }

    /// <summary>
    /// Validar datos del huésped con datos correctos
    /// </summary>
    [Fact]
    public void ValidarDatosHuesped_DatosValidos_NoLanzaExcepcion()
    {
        // Arrange
        var reserva = new Reserva 
        { 
            HuespedNombre = "Juan Pérez",
            HuespedDocumento = "12345678"
        };

        // Act
        var action = () => reserva.ValidarDatosHuesped();

        // Assert
        action.Should().NotThrow();
    }
}
