using Microsoft.EntityFrameworkCore;
using StayHub.Infrastructure.Out.Database.EfCore.Contexts;

namespace StayHub.UnitTests.Infrastructure;

/// <summary>
/// Factory para crear instancias de DbContext para testing con base de datos en memoria
/// </summary>
public static class TestDbContextFactory
{
    /// <summary>
    /// Crea un DbContext con base de datos en memoria para testing
    /// </summary>
    /// <param name="databaseName">Nombre único de la base de datos en memoria (opcional)</param>
    /// <returns>Instancia configurada de StayHubDbContext</returns>
    public static StayHubDbContext CreateInMemoryContext(string? databaseName = null)
    {
        var dbName = databaseName ?? Guid.NewGuid().ToString();

        var options = new DbContextOptionsBuilder<StayHubDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .EnableSensitiveDataLogging() // Para debugging en tests
            .Options;

        var context = new StayHubDbContext(options);

        // Asegurar que la base de datos esté creada
        context.Database.EnsureCreated();

        return context;
    }

    /// <summary>
    /// Crea un DbContext con datos de prueba pre-cargados
    /// </summary>
    /// <param name="databaseName">Nombre único de la base de datos en memoria (opcional)</param>
    /// <returns>Instancia de StayHubDbContext con datos de prueba</returns>
    public static StayHubDbContext CreateInMemoryContextWithData(string? databaseName = null)
    {
        var context = CreateInMemoryContext(databaseName);
        SeedTestData(context);
        return context;
    }

    /// <summary>
    /// Limpia y reestablece los datos de prueba en el contexto
    /// </summary>
    /// <param name="context">Contexto a limpiar y re-poblar</param>
    public static void ResetTestData(StayHubDbContext context)
    {
        // Limpiar datos existentes
        context.Reservas.RemoveRange(context.Reservas);
        context.Habitaciones.RemoveRange(context.Habitaciones);
        context.Hoteles.RemoveRange(context.Hoteles);
        context.SaveChanges();

        // Volver a poblar con datos de prueba
        SeedTestData(context);
    }

    /// <summary>
    /// Puebla la base de datos con datos de prueba consistentes
    /// </summary>
    /// <param name="context">Contexto a poblar</param>
    public static void SeedTestData(StayHubDbContext context)
    {
        // Solo agregar datos si no existen
        if (context.Hoteles.Any()) return;

        var hotels = TestDataSeeder.CreateTestHotels();
        context.Hoteles.AddRange(hotels);
        context.SaveChanges();

        var habitaciones = TestDataSeeder.CreateTestHabitaciones(hotels);
        context.Habitaciones.AddRange(habitaciones);
        context.SaveChanges();

        var reservas = TestDataSeeder.CreateTestReservas(hotels, habitaciones);
        context.Reservas.AddRange(reservas);
        context.SaveChanges();
    }
}