using StayHub.Domain.Entities;
using StayHub.Domain.Enums;
using StayHub.Infrastructure.In.Rest.Dtos;

namespace StayHub.Infrastructure.In.Rest.Mappers;

/// <summary>
/// Mapper para conversión entre Habitacion y DTOs
/// </summary>
public static class HabitacionMapper
{
    public static HabitacionDto ToDto(Habitacion habitacion)
    {
        return new HabitacionDto
        {
            HabitacionId = habitacion.HabitacionId,
            HotelId = habitacion.HotelId,
            NumeroHabitacion = habitacion.NumeroHabitacion,
            TipoHabitacion = habitacion.TipoHabitacion,
            Capacidad = habitacion.Capacidad,
            TarifaNoche = habitacion.TarifaNoche,
            Estado = habitacion.Estado.ToString()
        };
    }

    public static List<HabitacionDto> ToDtoList(IEnumerable<Habitacion> habitaciones)
    {
        return habitaciones.Select(ToDto).ToList();
    }

    public static Habitacion ToEntity(CreateHabitacionRequest request)
    {
        return new Habitacion
        {
            HotelId = request.HotelId,
            NumeroHabitacion = request.NumeroHabitacion,
            TipoHabitacion = request.TipoHabitacion,
            Capacidad = request.Capacidad,
            TarifaNoche = request.TarifaNoche,
            Estado = Estado.Activo
        };
    }

    public static Habitacion ToEntity(UpdateHabitacionRequest request)
    {
        return new Habitacion
        {
            HabitacionId = request.HabitacionId,
            NumeroHabitacion = request.NumeroHabitacion,
            TipoHabitacion = request.TipoHabitacion,
            Capacidad = request.Capacidad,
            TarifaNoche = request.TarifaNoche
        };
    }
}
