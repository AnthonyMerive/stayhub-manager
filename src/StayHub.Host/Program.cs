#region Using Statements
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StayHub.Application.Ports.In.Services;
using StayHub.Application.Ports.Out.Database;
using StayHub.Application.Ports.Out.Traceability;
using StayHub.Application.Services;
using StayHub.Infrastructure.In.Rest.Middlewares;
using StayHub.Infrastructure.Out.Database.EfCore.Adapters;
using StayHub.Infrastructure.Out.Database.EfCore.Contexts;
using StayHub.Infrastructure.Out.Traceability.Logging;
#endregion

#region Application Builder Initialization
var builder = WebApplication.CreateBuilder(args);
#endregion

#region Logging Configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Suprimir logs de excepciones por defecto de ASP.NET Core
builder.Logging.AddFilter("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware", LogLevel.None);
builder.Logging.AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.Warning);

// Suprimir TODOS los logs de Entity Framework Core
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database", LogLevel.None);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Connection", LogLevel.None);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Transaction", LogLevel.None);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.None);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Migrations", LogLevel.None);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Model", LogLevel.None);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Query", LogLevel.None);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Update", LogLevel.None);
#endregion

#region Services Configuration
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StayHub Manager API",
        Version = "v1",
        Description = "API para gestión de hoteles y reservas",
        Contact = new OpenApiContact
        {
            Name = "StayHub Aviatur",
            Email = "support@aviatur.com"
        }
    });
});

// Controllers
builder.Services.AddControllers();

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StayHubDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    }));
#endregion

#region Dependency Injection
// Repositorios
builder.Services.AddScoped<IHotelRepository, HotelEfAdapter>();
builder.Services.AddScoped<IHabitacionRepository, HabitacionEfAdapter>();
builder.Services.AddScoped<IReservaRepository, ReservaEfAdapter>();

// Servicios
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IHabitacionService, HabitacionService>();
builder.Services.AddScoped<IReservaService, ReservaService>();

// Traceability Adapter
builder.Services.AddScoped<ITraceability, TraceabilityAdapter>();
#endregion

#region Health Checks
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString ?? "", name: "sqlserver", tags: ["db", "sql"]);
#endregion

#region App Builder
var app = builder.Build();
#endregion

#region HTTP Request Pipeline
// Middleware global de manejo de excepciones
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Swagger (en desarrollo)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "StayHub Manager API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
#endregion

#region Endpoints Mapping
// Mapear controllers
app.MapControllers();

// Health checks
app.MapHealthChecks("/healthz/ready");
app.MapHealthChecks("/healthz/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});

// Endpoint de versión
app.MapGet("/version", () => new
{
    Version = Environment.GetEnvironmentVariable("VERSION") ?? "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow
});
#endregion

#region Application Startup
await app.RunAsync();
#endregion
