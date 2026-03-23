using StayHub.Domain.Entities;
using StayHub.Domain.Enums;

namespace StayHub.UnitTests.Infrastructure;

/// <summary>
/// Clase para crear datos de prueba consistentes
/// </summary>
public static class TestDataSeeder
{
    /// <summary>
    /// Crea hoteles de prueba
    /// </summary>
    public static List<Hotel> CreateTestHotels()
    {
        return new List<Hotel>
        {
            new Hotel
            {
                HotelId = 1,
                Nombre = "Hotel Luxury Plaza",
                Ciudad = "Bogotá",
                Direccion = "Calle 100 #15-20",
                Estado = Estado.Activo,
                FechaCreacion = DateTime.UtcNow.AddDays(-30)
            },
            new Hotel
            {
                HotelId = 2,
                Nombre = "Hotel Business Center",
                Ciudad = "Medellín",
                Direccion = "Carrera 43A #1-50",
                Estado = Estado.Activo,
                FechaCreacion = DateTime.UtcNow.AddDays(-20)
            },
            new Hotel
            {
                HotelId = 3,
                Nombre = "Hotel Desactivado",
                Ciudad = "Cali",
                Direccion = "Avenida 6N #15N-23",
                Estado = Estado.Inactivo,
                FechaCreacion = DateTime.UtcNow.AddDays(-60)
            }
        };
    }

    /// <summary>
    /// Crea habitaciones de prueba
    /// </summary>
    public static List<Habitacion> CreateTestHabitaciones(List<Hotel> hotels)
    {
        return new List<Habitacion>
        {
            // Hotel Luxury Plaza (ID: 1)
            new Habitacion
            {
                HabitacionId = 1,
                HotelId = 1,
                NumeroHabitacion = "101",
                TipoHabitacion = "Sencilla",
                Capacidad = 2,
                TarifaNoche = 150000m,
                Estado = Estado.Activo
            },
            new Habitacion
            {
                HabitacionId = 2,
                HotelId = 1,
                NumeroHabitacion = "102",
                TipoHabitacion = "Doble",
                Capacidad = 4,
                TarifaNoche = 250000m,
                Estado = Estado.Activo
            },
            new Habitacion
            {
                HabitacionId = 3,
                HotelId = 1,
                NumeroHabitacion = "201",
                TipoHabitacion = "Suite",
                Capacidad = 6,
                TarifaNoche = 400000m,
                Estado = Estado.Activo
            },

            // Hotel Business Center (ID: 2)
            new Habitacion
            {
                HabitacionId = 4,
                HotelId = 2,
                NumeroHabitacion = "301",
                TipoHabitacion = "Ejecutiva",
                Capacidad = 2,
                TarifaNoche = 180000m,
                Estado = Estado.Activo
            },
            new Habitacion
            {
                HabitacionId = 5,
                HotelId = 2,
                NumeroHabitacion = "302",
                TipoHabitacion = "Doble",
                Capacidad = 4,
                TarifaNoche = 280000m,
                Estado = Estado.Inactivo // Para probar validación BR-04
            },

            // Hotel Desactivado (ID: 3)
            new Habitacion
            {
                HabitacionId = 6,
                HotelId = 3,
                NumeroHabitacion = "401",
                TipoHabitacion = "Sencilla",
                Capacidad = 2,
                TarifaNoche = 120000m,
                Estado = Estado.Activo
            }
        };
    }

    /// <summary>
    /// Crea reservas de prueba
    /// </summary>
    public static List<Reserva> CreateTestReservas(List<Hotel> hotels, List<Habitacion> habitaciones)
    {
        var baseDate = DateTime.UtcNow.Date;

        return new List<Reserva>
        {
            // Reserva activa en Hotel Luxury Plaza
            new Reserva
            {
                ReservaId = 1,
                HotelId = 1,
                HabitacionId = 1,
                HuespedNombre = "Juan Pérez",
                HuespedDocumento = "12345678",
                FechaEntrada = baseDate.AddDays(10),
                FechaSalida = baseDate.AddDays(13), // 3 noches
                CantidadHuespedes = 2,
                ValorNoche = 150000m,
                TotalReserva = 450000m, // 3 noches × 150,000
                EstadoReserva = EstadoReserva.Activa,
                FechaCreacion = DateTime.UtcNow.AddDays(-5)
            },

            // Reserva para validar overbooking
            new Reserva
            {
                ReservaId = 2,
                HotelId = 1,
                HabitacionId = 2,
                HuespedNombre = "María García",
                HuespedDocumento = "87654321",
                FechaEntrada = baseDate.AddDays(15),
                FechaSalida = baseDate.AddDays(18), // 3 noches
                CantidadHuespedes = 3,
                ValorNoche = 250000m,
                TotalReserva = 750000m,
                EstadoReserva = EstadoReserva.Activa,
                FechaCreacion = DateTime.UtcNow.AddDays(-3)
            },

            // Reserva cancelada
            new Reserva
            {
                ReservaId = 3,
                HotelId = 2,
                HabitacionId = 4,
                HuespedNombre = "Carlos Rodríguez",
                HuespedDocumento = "11223344",
                FechaEntrada = baseDate.AddDays(5),
                FechaSalida = baseDate.AddDays(7), // 2 noches
                CantidadHuespedes = 1,
                ValorNoche = 180000m,
                TotalReserva = 360000m,
                EstadoReserva = EstadoReserva.Cancelada,
                FechaCreacion = DateTime.UtcNow.AddDays(-10)
            }
        };
    }

    /// <summary>
    /// Crear datos mínimos para pruebas específicas
    /// </summary>
    public static (Hotel hotel, Habitacion habitacion) CreateMinimalTestData()
    {
        var hotel = new Hotel
        {
            HotelId = 1,
            Nombre = "Hotel Test",
            Ciudad = "Bogotá",
            Direccion = "Dirección Test",
            Estado = Estado.Activo,
            FechaCreacion = DateTime.UtcNow
        };

        var habitacion = new Habitacion
        {
            HabitacionId = 1,
            HotelId = 1,
            NumeroHabitacion = "101",
            TipoHabitacion = "Test",
            Capacidad = 2,
            TarifaNoche = 100000m,
            Estado = Estado.Activo
        };

        return (hotel, habitacion);
    }
}