using FluentAssertions;
using StayHub.Domain.Entities;
using StayHub.Domain.Enums;
using StayHub.Infrastructure.Out.Database.EfCore.Adapters;
using StayHub.UnitTests.Infrastructure;
using Xunit;

namespace StayHub.UnitTests.Integration;

/// <summary>
/// Tests de integración para ReservaRepository usando base de datos en memoria
/// </summary>
public class ReservaRepositoryIntegrationTests : IntegrationTestBase
{
    private readonly ReservaEfAdapter _repository;

    public ReservaRepositoryIntegrationTests() : base(seedData: true)
    {
        _repository = new ReservaEfAdapter(Context);
    }

    [Fact]
    public async Task GetByIdAsync_ReservaExistente_DebeRetornarReserva()
    {
        // Arrange
        var reservaId = 1; // De los datos de prueba

        // Act
        var resultado = await _repository.GetByIdAsync(reservaId);

        // Assert
        resultado.Success.Should().BeTrue();
        resultado.Data.Should().NotBeNull();
        resultado.Data!.ReservaId.Should().Be(reservaId);
        resultado.Data.HuespedNombre.Should().Be("Juan Pérez");
        resultado.Data.HuespedDocumento.Should().Be("12345678");
    }

    [Fact]
    public async Task GetByIdAsync_ReservaNoExistente_DebeRetornarNull()
    {
        // Arrange
        var reservaId = 999;

        // Act
        var resultado = await _repository.GetByIdAsync(reservaId);

        // Assert
        resultado.Success.Should().BeTrue();
        resultado.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetByHotelIdAsync_HotelConReservas_DebeRetornarReservas()
    {
        // Arrange
        var hotelId = 1; // Hotel Luxury Plaza tiene reservas

        // Act
        var resultado = await _repository.GetByHotelIdAsync(hotelId);

        // Assert
        resultado.Success.Should().BeTrue();
        resultado.Data.Should().NotBeEmpty();
        resultado.Data.Should().HaveCount(2); // Según datos de prueba
        resultado.Data.Should().OnlyContain(r => r.HotelId == hotelId);
    }

    [Fact]
    public async Task GetByHotelIdAsync_HotelSinReservas_DebeRetornarListaVacia()
    {
        // Arrange
        var hotelId = 3; // Hotel desactivado no tiene reservas activas

        // Act
        var resultado = await _repository.GetByHotelIdAsync(hotelId);

        // Assert
        resultado.Success.Should().BeTrue();
        resultado.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task ExisteOverbookingAsync_ConFlictoFechas_DebeRetornarTrue()
    {
        // Arrange - Usar fechas que se solapan con reserva existente
        var habitacionId = 1;
        var fechaEntrada = DateTime.UtcNow.Date.AddDays(11); // Se solapa con reserva existente (10-13)
        var fechaSalida = DateTime.UtcNow.Date.AddDays(14);

        // Act
        var resultado = await _repository.ExisteOverbookingAsync(habitacionId, fechaEntrada, fechaSalida);

        // Assert
        resultado.Success.Should().BeTrue();
        resultado.Data.Should().BeTrue();
    }

    [Fact]
    public async Task ExisteOverbookingAsync_SinConflictoFechas_DebeRetornarFalse()
    {
        // Arrange - Usar fechas que NO se solapan
        var habitacionId = 1;
        var fechaEntrada = DateTime.UtcNow.Date.AddDays(20);
        var fechaSalida = DateTime.UtcNow.Date.AddDays(23);

        // Act
        var resultado = await _repository.ExisteOverbookingAsync(habitacionId, fechaEntrada, fechaSalida);

        // Assert
        resultado.Success.Should().BeTrue();
        resultado.Data.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAsync_ReservaNueva_DebeCrearCorrectamente()
    {
        // Arrange - Usar IDs únicos para evitar conflictos de tracking
        var hotel = new Hotel
        {
            HotelId = 100, // ID único para este test
            Nombre = "Hotel Test",
            Ciudad = "Test City",
            Direccion = "Test Address",
            Estado = Estado.Activo,
            FechaCreacion = DateTime.UtcNow
        };

        var habitacion = new Habitacion
        {
            HabitacionId = 100, // ID único para este test
            HotelId = 100,
            NumeroHabitacion = "TEST-101",
            TipoHabitacion = "Test",
            Capacidad = 2,
            TarifaNoche = 100000m,
            Estado = Estado.Activo
        };

        // Agregar hotel y habitación al contexto
        Context.Hoteles.Add(hotel);
        Context.Habitaciones.Add(habitacion);
        await Context.SaveChangesAsync();

        var nuevaReserva = new Reserva
        {
            HotelId = hotel.HotelId,
            HabitacionId = habitacion.HabitacionId,
            HuespedNombre = "Test User",
            HuespedDocumento = "12345678",
            FechaEntrada = DateTime.UtcNow.Date.AddDays(1),
            FechaSalida = DateTime.UtcNow.Date.AddDays(3),
            CantidadHuespedes = 2,
            ValorNoche = 100000m,
            TotalReserva = 200000m,
            EstadoReserva = EstadoReserva.Activa
        };

        // Act
        var resultado = await _repository.CreateAsync(nuevaReserva);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Success.Should().BeTrue();
        resultado.Data!.ReservaId.Should().BeGreaterThan(0);
        resultado.Data.HuespedNombre.Should().Be("Test User");

        // Verificar que se guardó en la base de datos
        var reservaEnDb = await Context.Reservas.FindAsync(resultado.Data.ReservaId);
        reservaEnDb.Should().NotBeNull();
    }

    [Fact]
    public async Task CancelarAsync_ReservaExistente_DebeActualizarEstado()
    {
        // Arrange
        var reservaId = 1; // Reserva activa de los datos de prueba

        // Act
        var resultado = await _repository.CancelarAsync(reservaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Success.Should().BeTrue();

        var reserva = await Context.Reservas.FindAsync(reservaId);
        reserva.Should().NotBeNull();
        reserva!.EstadoReserva.Should().Be(EstadoReserva.Cancelada);
    }

    [Fact]
    public async Task GetPaginatedAsync_ConParametros_DebeRetornarPaginacionCorrecta()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 2;

        // Act
        var resultado = await _repository.GetPaginatedAsync(pageNumber, pageSize);

        // Assert
        resultado.Success.Should().BeTrue();
        resultado.Data.Should().NotBeNull();
        resultado!.Data!.Items.Should().HaveCount(2); // PageSize = 2
        resultado.Data.PageNumber.Should().Be(1);
        resultado.Data.PageSize.Should().Be(2);
        resultado.Data.TotalRecords.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetPaginatedAsync_FiltradoPorHotel_DebeRetornarSoloReservasDelHotel()
    {
        // Arrange
        var hotelId = 1;
        var pageNumber = 1;
        var pageSize = 10;

        // Act
        var resultado = await _repository.GetPaginatedAsync(pageNumber, pageSize, hotelId);

        // Assert
        resultado.Success.Should().BeTrue();
        resultado.Data.Should().NotBeNull();
        resultado!.Data!.Items.Should().OnlyContain(r => r.HotelId == hotelId);
    }
}