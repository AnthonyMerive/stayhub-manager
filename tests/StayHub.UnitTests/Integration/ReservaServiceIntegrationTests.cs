using FluentAssertions;
using Moq;
using StayHub.Application.Ports.Out.Traceability;
using StayHub.Application.Rules.Constants;
using StayHub.Application.Services;
using StayHub.Domain.Entities;
using StayHub.Domain.Enums;
using StayHub.Domain.Exceptions;
using StayHub.Infrastructure.Out.Database.EfCore.Adapters;
using StayHub.UnitTests.Infrastructure;
using Xunit;

// Importaciones específicas para las constantes
using StayHub.Domain.Constants.Reserva;

namespace StayHub.UnitTests.Integration;

/// <summary>
/// Tests de integración para ReservaService usando base de datos en memoria
/// </summary>
public class ReservaServiceIntegrationTests : IntegrationTestBase
{
    private readonly ReservaService _service;
    private readonly ReservaEfAdapter _reservaRepository;
    private readonly HabitacionEfAdapter _habitacionRepository;
    private readonly Mock<ITraceability> _mockTraceabilityAdapter;

    public ReservaServiceIntegrationTests() : base(seedData: true)
    {
        _reservaRepository = new ReservaEfAdapter(Context);
        _habitacionRepository = new HabitacionEfAdapter(Context);
        _mockTraceabilityAdapter = new Mock<ITraceability>();

        _service = new ReservaService(
            _reservaRepository,
            _habitacionRepository,
            _mockTraceabilityAdapter.Object);
    }

    [Fact]
    public async Task CreateAsync_ReservaValida_DebeCrearExitosamente()
    {
        // Arrange
        var reserva = new Reserva
        {
            HotelId = 1, // Hotel activo de los datos de prueba
            HabitacionId = 3, // Habitación Suite disponible
            HuespedNombre = "Ana López",
            HuespedDocumento = "98765432",
            FechaEntrada = DateTime.UtcNow.Date.AddDays(25),
            FechaSalida = DateTime.UtcNow.Date.AddDays(28),
            CantidadHuespedes = 4
        };

        // Act
        var resultado = await _service.CreateAsync(reserva, "test-transaction-id");

        // Assert
        resultado.Should().NotBeNull();
        resultado.ReservaId.Should().BeGreaterThan(0);
        resultado.TotalReserva.Should().Be(1200000m); // 3 noches × 400,000
        resultado.EstadoReserva.Should().Be(EstadoReserva.Activa);

        // Verificar que se guardó en la base de datos
        var reservaEnDb = await _reservaRepository.GetByIdAsync(resultado.ReservaId);
        reservaEnDb.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_HotelNoExiste_DebeLanzarEntityNotFoundException()
    {
        // Arrange
        var reserva = new Reserva
        {
            HotelId = 999, // Hotel que no existe
            HabitacionId = 999, // También habitación que no existe para evitar validaciones anteriores
            HuespedNombre = "Test User",
            HuespedDocumento = "12345678",
            FechaEntrada = DateTime.UtcNow.Date.AddDays(1),
            FechaSalida = DateTime.UtcNow.Date.AddDays(3),
            CantidadHuespedes = 1
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.CreateAsync(reserva, "test-transaction-id"));

        exception.EntityName.Should().Be(ReservaEntityNames.Habitacion);
        exception.EntityId.Should().Be(999);
    }

    [Fact]
    public async Task CreateAsync_HabitacionNoExiste_DebeLanzarEntityNotFoundException()
    {
        // Arrange
        var reserva = new Reserva
        {
            HotelId = 1, // Hotel válido
            HabitacionId = 999, // Habitación que no existe
            HuespedNombre = "Test User",
            HuespedDocumento = "12345678",
            FechaEntrada = DateTime.UtcNow.Date.AddDays(1),
            FechaSalida = DateTime.UtcNow.Date.AddDays(3),
            CantidadHuespedes = 1
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.CreateAsync(reserva, "test-transaction-id"));

        exception.EntityName.Should().Be(ReservaEntityNames.Habitacion);
        exception.EntityId.Should().Be(999);
    }

    [Fact]
    public async Task CreateAsync_HabitacionInactiva_DebeLanzarBusinessRuleException()
    {
        // Arrange
        var reserva = new Reserva
        {
            HotelId = 2,
            HabitacionId = 5, // Habitación inactiva según datos de prueba
            HuespedNombre = "Test User",
            HuespedDocumento = "12345678",
            FechaEntrada = DateTime.UtcNow.Date.AddDays(1),
            FechaSalida = DateTime.UtcNow.Date.AddDays(3),
            CantidadHuespedes = 2
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _service.CreateAsync(reserva, "test-transaction-id"));

        exception.RuleCode.Should().Be(ReservaErrorCodes.HabitacionInactiva);
        exception.Message.Should().Be(ReservaErrorMessages.HabitacionInactiva);
    }

    [Fact]
    public async Task CreateAsync_FechaSalidaMenorQueEntrada_DebeLanzarBusinessRuleException()
    {
        // Arrange
        var reserva = new Reserva
        {
            HotelId = 1,
            HabitacionId = 1,
            HuespedNombre = "Test User",
            HuespedDocumento = "12345678",
            FechaEntrada = DateTime.UtcNow.Date.AddDays(5),
            FechaSalida = DateTime.UtcNow.Date.AddDays(3), // Fecha anterior
            CantidadHuespedes = 1
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _service.CreateAsync(reserva, "test-transaction-id"));

        exception.RuleCode.Should().Be("INVALID_CHECK_OUT_DATE");
    }

    [Fact]
    public async Task CreateAsync_ExcedeCapacidad_DebeLanzarBusinessRuleException()
    {
        // Arrange
        var reserva = new Reserva
        {
            HotelId = 1,
            HabitacionId = 1, // Habitación con capacidad 2
            HuespedNombre = "Test User",
            HuespedDocumento = "12345678",
            FechaEntrada = DateTime.UtcNow.Date.AddDays(1),
            FechaSalida = DateTime.UtcNow.Date.AddDays(3),
            CantidadHuespedes = 5 // Excede capacidad de 2
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _service.CreateAsync(reserva, "test-transaction-id"));

        exception.RuleCode.Should().Be(ReservaErrorCodes.CapacidadExcedida);
        exception.Message.Should().Contain("5").And.Contain("2");
    }

    [Fact]
    public async Task CreateAsync_Overbooking_DebeLanzarBusinessRuleException()
    {
        // Arrange - Usar fechas que se solapan con reserva existente
        var reserva = new Reserva
        {
            HotelId = 1,
            HabitacionId = 1, // Habitación que ya tiene reserva del 10-13
            HuespedNombre = "Test User",
            HuespedDocumento = "12345678",
            FechaEntrada = DateTime.UtcNow.Date.AddDays(11), // Se solapa
            FechaSalida = DateTime.UtcNow.Date.AddDays(14),
            CantidadHuespedes = 1
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _service.CreateAsync(reserva, "test-transaction-id"));

        exception.RuleCode.Should().Be(ReservaErrorCodes.OverbookingDetected);
        exception.Message.Should().Be(ReservaErrorMessages.OverbookingDetected);
    }

    [Fact]
    public async Task CreateAsync_NombreHuespedVacio_DebeLanzarBusinessRuleException()
    {
        // Arrange
        var reserva = new Reserva
        {
            HotelId = 1,
            HabitacionId = 1,
            HuespedNombre = "", // Nombre vacío
            HuespedDocumento = "12345678",
            FechaEntrada = DateTime.UtcNow.Date.AddDays(1),
            FechaSalida = DateTime.UtcNow.Date.AddDays(3),
            CantidadHuespedes = 1
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _service.CreateAsync(reserva, "test-transaction-id"));

        exception.RuleCode.Should().Be("NOMBRE_HUESPED_REQUIRED");
    }

    [Fact]
    public async Task CancelarAsync_ReservaExistente_DebeActualizarEstado()
    {
        // Arrange
        var reservaId = 1; // Reserva activa de los datos de prueba

        // Act
        await _service.CancelarAsync(reservaId, "test-transaction-id");

        // Assert
        var reserva = await _reservaRepository.GetByIdAsync(reservaId);
        reserva.Success.Should().BeTrue();
        reserva.Data.Should().NotBeNull();
        reserva.Data!.EstadoReserva.Should().Be(EstadoReserva.Cancelada);
    }

    [Fact]
    public async Task CancelarAsync_ReservaNoExiste_DebeLanzarEntityNotFoundException()
    {
        // Arrange
        var reservaId = 999;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.CancelarAsync(reservaId, "test-transaction-id"));

        exception.EntityName.Should().Be(ReservaEntityNames.Reserva);
        exception.EntityId.Should().Be(999);
    }

    [Fact]
    public async Task CancelarAsync_ReservaYaCancelada_DebeLanzarBusinessRuleException()
    {
        // Arrange
        var reservaId = 3; // Reserva cancelada según datos de prueba

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _service.CancelarAsync(reservaId, "test-transaction-id"));

        exception.RuleCode.Should().Be(ReservaErrorCodes.ReservaYaCancelada);
        exception.Message.Should().Be(ReservaErrorMessages.ReservaYaCancelada);
    }

    [Fact]
    public async Task GetByIdAsync_ReservaExistente_DebeRetornarReserva()
    {
        // Arrange
        var reservaId = 1;

        // Act
        var resultado = await _service.GetByIdAsync(reservaId, "test-transaction-id");

        // Assert
        resultado.Should().NotBeNull();
        resultado!.ReservaId.Should().Be(reservaId);
    }

    [Fact]
    public async Task GetByHotelIdAsync_HotelConReservas_DebeRetornarReservas()
    {
        // Arrange
        var hotelId = 1;

        // Act
        var resultado = await _service.GetByHotelIdAsync(hotelId, "test-transaction-id");

        // Assert
        resultado.Should().NotBeEmpty();
        resultado.Should().OnlyContain(r => r.HotelId == hotelId);
    }
}