using StayHub.Domain.Entities;
using StayHub.Domain.Enums;
using StayHub.Infrastructure.In.Rest.Dtos;

namespace StayHub.Infrastructure.In.Rest.Mappers;

/// <summary>
/// Mapper para conversión entre Reserva y DTOs
/// </summary>
public static class ReservaMapper
{
    public static ReservaDto ToDto(Reserva reserva)
    {
        return new ReservaDto
        {
            ReservaId = reserva.ReservaId,
            HotelId = reserva.HotelId,
            HabitacionId = reserva.HabitacionId,
            HuespedNombre = reserva.HuespedNombre,
            HuespedDocumento = reserva.HuespedDocumento,
            FechaEntrada = reserva.FechaEntrada,
            FechaSalida = reserva.FechaSalida,
            CantidadHuespedes = reserva.CantidadHuespedes,
            ValorNoche = reserva.ValorNoche,
            TotalReserva = reserva.TotalReserva,
            EstadoReserva = reserva.EstadoReserva.ToString(),
            FechaCreacion = reserva.FechaCreacion,
            NochesEstadia = reserva.ObtenerNochesEstadia()
        };
    }

    public static List<ReservaDto> ToDtoList(IEnumerable<Reserva> reservas)
    {
        return reservas.Select(ToDto).ToList();
    }

    public static Reserva ToEntity(CreateReservaRequest request)
    {
        return new Reserva
        {
            HotelId = request.HotelId,
            HabitacionId = request.HabitacionId,
            HuespedNombre = request.HuespedNombre,
            HuespedDocumento = request.HuespedDocumento,
            FechaEntrada = request.FechaEntrada,
            FechaSalida = request.FechaSalida,
            CantidadHuespedes = request.CantidadHuespedes,
            EstadoReserva = EstadoReserva.Activa,
            FechaCreacion = DateTime.UtcNow
        };
    }
}
