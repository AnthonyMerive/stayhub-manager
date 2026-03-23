using StayHub.Domain.Entities;
using StayHub.Domain.Enums;
using StayHub.Infrastructure.In.Rest.Dtos;

namespace StayHub.Infrastructure.In.Rest.Mappers;

/// <summary>
/// Mapper para conversión entre Hotel y DTOs
/// </summary>
public static class HotelMapper
{
    public static HotelDto ToDto(Hotel hotel)
    {
        return new HotelDto
        {
            HotelId = hotel.HotelId,
            Nombre = hotel.Nombre,
            Ciudad = hotel.Ciudad,
            Direccion = hotel.Direccion,
            Estado = hotel.Estado.ToString(),
            FechaCreacion = hotel.FechaCreacion
        };
    }

    public static List<HotelDto> ToDtoList(IEnumerable<Hotel> hoteles)
    {
        return hoteles.Select(ToDto).ToList();
    }

    public static Hotel ToEntity(CreateHotelRequest request)
    {
        return new Hotel
        {
            Nombre = request.Nombre,
            Ciudad = request.Ciudad,
            Direccion = request.Direccion,
            Estado = Estado.Activo,
            FechaCreacion = DateTime.UtcNow
        };
    }

    public static Hotel ToEntity(UpdateHotelRequest request)
    {
        return new Hotel
        {
            HotelId = request.HotelId,
            Nombre = request.Nombre,
            Ciudad = request.Ciudad,
            Direccion = request.Direccion
        };
    }
}
