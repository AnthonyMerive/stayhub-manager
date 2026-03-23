using Microsoft.EntityFrameworkCore;
using StayHub.Domain.Entities;
using StayHub.Domain.Enums;
using StayHub.Domain.Exceptions;
using StayHub.Infrastructure.Out.Database.EfCore.Contexts;

namespace StayHub.UnitTests.Infrastructure;

/// <summary>
/// Helpers para verificar datos en tests de integración
/// </summary>
public static class DatabaseTestHelpers
{
    /// <summary>
    /// Verifica que una reserva exista en la base de datos con los datos esperados
    /// </summary>
    public static async Task<Reserva> VerifyReservaExists(StayHubDbContext context, int reservaId)
    {
        var reserva = await context.Reservas
            .Include(r => r.Hotel)
            .Include(r => r.Habitacion)
            .FirstOrDefaultAsync(r => r.ReservaId == reservaId);

        return reserva ?? throw new DatabaseException($"Reserva con ID {reservaId} no encontrada en la base de datos");
    }

    /// <summary>
    /// Verifica que una reserva tenga el estado esperado
    /// </summary>
    public static async Task VerifyReservaStatus(StayHubDbContext context, int reservaId, EstadoReserva estadoEsperado)
    {
        var reserva = await context.Reservas.FindAsync(reservaId) ?? throw new DatabaseException($"Reserva con ID {reservaId} no encontrada");
        if (reserva.EstadoReserva != estadoEsperado)
            throw new DatabaseException($"Reserva {reservaId} tiene estado {reserva.EstadoReserva}, esperado {estadoEsperado}");
    }

    /// <summary>
    /// Cuenta el número de reservas activas para una habitación en un rango de fechas
    /// </summary>
    public static async Task<int> CountActiveReservasInDateRange(
        StayHubDbContext context, 
        int habitacionId, 
        DateTime fechaInicio, 
        DateTime fechaFin)
    {
        return await context.Reservas
            .Where(r => r.HabitacionId == habitacionId)
            .Where(r => r.EstadoReserva == EstadoReserva.Activa)
            .Where(r => r.FechaEntrada < fechaFin && r.FechaSalida > fechaInicio)
            .CountAsync();
    }

    /// <summary>
    /// Obtiene todas las reservas de un hotel ordenadas por fecha
    /// </summary>
    public static async Task<List<Reserva>> GetReservasByHotel(StayHubDbContext context, int hotelId)
    {
        return await context.Reservas
            .Include(r => r.Habitacion)
            .Where(r => r.HotelId == hotelId)
            .OrderBy(r => r.FechaEntrada)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica que no existan reservas con solapamiento de fechas para una habitación
    /// </summary>
    public static async Task<bool> HasDateOverlap(
        StayHubDbContext context, 
        int habitacionId, 
        DateTime fechaEntrada, 
        DateTime fechaSalida)
    {
        return await context.Reservas
            .Where(r => r.HabitacionId == habitacionId)
            .Where(r => r.EstadoReserva == EstadoReserva.Activa)
            .Where(r => r.FechaEntrada < fechaSalida && r.FechaSalida > fechaEntrada)
            .AnyAsync();
    }

    /// <summary>
    /// Limpia todas las reservas de la base de datos (útil para tests)
    /// </summary>
    public static async Task ClearAllReservas(StayHubDbContext context)
    {
        var reservas = await context.Reservas.ToListAsync();
        context.Reservas.RemoveRange(reservas);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Crea una reserva de prueba rápidamente
    /// </summary>
    public static async Task<Reserva> CreateTestReserva(
        StayHubDbContext context,
        int hotelId = 1,
        int habitacionId = 1,
        DateTime? fechaEntrada = null,
        DateTime? fechaSalida = null,
        string huespedNombre = "Test User",
        string huespedDocumento = "12345678",
        int cantidadHuespedes = 2,
        EstadoReserva estado = EstadoReserva.Activa)
    {
        var reserva = new Reserva
        {
            HotelId = hotelId,
            HabitacionId = habitacionId,
            HuespedNombre = huespedNombre,
            HuespedDocumento = huespedDocumento,
            FechaEntrada = fechaEntrada ?? DateTime.UtcNow.Date.AddDays(1),
            FechaSalida = fechaSalida ?? DateTime.UtcNow.Date.AddDays(3),
            CantidadHuespedes = cantidadHuespedes,
            ValorNoche = 150000m,
            TotalReserva = 300000m, // 2 noches × 150,000
            EstadoReserva = estado,
            FechaCreacion = DateTime.UtcNow
        };

        context.Reservas.Add(reserva);
        await context.SaveChangesAsync();

        return reserva;
    }

    /// <summary>
    /// Verifica que la base de datos tenga la cantidad esperada de registros
    /// </summary>
    public static async Task VerifyDatabaseCounts(
        StayHubDbContext context, 
        int expectedHotels, 
        int expectedHabitaciones, 
        int expectedReservas)
    {
        var hotelCount = await context.Hoteles.CountAsync();
        var habitacionCount = await context.Habitaciones.CountAsync();
        var reservaCount = await context.Reservas.CountAsync();

        if (hotelCount != expectedHotels)
            throw new DatabaseException($"Esperados {expectedHotels} hoteles, encontrados {hotelCount}");

        if (habitacionCount != expectedHabitaciones)
            throw new DatabaseException($"Esperadas {expectedHabitaciones} habitaciones, encontradas {habitacionCount}");

        if (reservaCount != expectedReservas)
            throw new DatabaseException($"Esperadas {expectedReservas} reservas, encontradas {reservaCount}");
    }
}