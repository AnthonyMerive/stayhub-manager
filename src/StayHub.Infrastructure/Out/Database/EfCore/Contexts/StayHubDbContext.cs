using Microsoft.EntityFrameworkCore;
using StayHub.Domain.Entities;

namespace StayHub.Infrastructure.Out.Database.EfCore.Contexts;

/// <summary>
/// DbContext principal de la aplicación
/// </summary>
public class StayHubDbContext(DbContextOptions<StayHubDbContext> options) : DbContext(options)
{
    public DbSet<Hotel> Hoteles => Set<Hotel>();
    public DbSet<Habitacion> Habitaciones => Set<Habitacion>();
    public DbSet<Reserva> Reservas => Set<Reserva>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Hotel
        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.ToTable("Hoteles");
            entity.HasKey(e => e.HotelId);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Ciudad).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Direccion).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETUTCDATE()");
        });

        // Configuración de Habitacion
        modelBuilder.Entity<Habitacion>(entity =>
        {
            entity.ToTable("Habitaciones");
            entity.HasKey(e => e.HabitacionId);
            entity.Property(e => e.NumeroHabitacion).IsRequired().HasMaxLength(10);
            entity.Property(e => e.TipoHabitacion).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TarifaNoche).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Hotel)
                .WithMany(h => h.Habitaciones)
                .HasForeignKey(e => e.HotelId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de Reserva
        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.ToTable("Reservas");
            entity.HasKey(e => e.ReservaId);
            entity.Property(e => e.HuespedNombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.HuespedDocumento).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ValorNoche).HasPrecision(18, 2);
            entity.Property(e => e.TotalReserva).HasPrecision(18, 2);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Hotel)
                .WithMany(h => h.Reservas)
                .HasForeignKey(e => e.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Habitacion)
                .WithMany(h => h.Reservas)
                .HasForeignKey(e => e.HabitacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice para consultas de overbooking
            entity.HasIndex(e => new { e.HabitacionId, e.FechaEntrada, e.FechaSalida });
        });
    }
}
