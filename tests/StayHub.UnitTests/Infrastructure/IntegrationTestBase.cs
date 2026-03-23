using Microsoft.Extensions.Logging;
using Moq;
using StayHub.Infrastructure.Out.Database.EfCore.Contexts;

namespace StayHub.UnitTests.Infrastructure;

/// <summary>
/// Clase base para tests de integración con base de datos en memoria
/// </summary>
public abstract class IntegrationTestBase : IDisposable
{
    protected StayHubDbContext Context { get; private set; }
    protected Mock<ILogger<T>> CreateMockLogger<T>() => new Mock<ILogger<T>>();

    protected IntegrationTestBase()
    {
        Context = TestDbContextFactory.CreateInMemoryContext();
    }

    protected IntegrationTestBase(bool seedData)
    {
        Context = seedData 
            ? TestDbContextFactory.CreateInMemoryContextWithData()
            : TestDbContextFactory.CreateInMemoryContext();
    }

    /// <summary>
    /// Limpia y restablece los datos de prueba
    /// </summary>
    protected void ResetData()
    {
        TestDbContextFactory.ResetTestData(Context);
    }

    /// <summary>
    /// Crea un contexto completamente nuevo para el test
    /// </summary>
    protected void RecreateContext()
    {
        Context.Dispose();
        Context = TestDbContextFactory.CreateInMemoryContextWithData();
    }

    /// <summary>
    /// Obtiene el contexto actual para operaciones manuales
    /// </summary>
    protected StayHubDbContext GetContext() => Context;

    public virtual void Dispose()
    {
        Context?.Dispose();
        GC.SuppressFinalize(this);
    }
}