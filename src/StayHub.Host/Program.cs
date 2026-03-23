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

var builder = WebApplication.CreateBuilder(args);

// ============================================
// Configuración de Logging
// ============================================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Suprimir logs de excepciones por defecto de ASP.NET Core
builder.Logging.AddFilter("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware", LogLevel.None);
builder.Logging.AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.Warning);

// ============================================
// Configuración de servicios
// ============================================

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

// ============================================
// Inyección de dependencias
// ============================================

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

// ============================================
// Health Checks
// ============================================
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString ?? "", name: "sqlserver", tags: ["db", "sql"]);

var app = builder.Build();

// ============================================
// Pipeline HTTP
// ============================================

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

// ============================================
// Ejecutar aplicación
// ============================================
await app.RunAsync();
