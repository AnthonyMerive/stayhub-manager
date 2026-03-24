using StayHub.Domain.Entities;
using StayHub.Shared.Types;

namespace StayHub.Application.Ports.In.Services;

/// <summary>
/// Puerto de entrada para operaciones de hoteles
/// </summary>
public interface IHotelService
{
    /// <summary>
    /// Obtiene un hotel por su identificador único
    /// </summary>
    /// <param name="hotelId">Identificador único del hotel</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>El hotel encontrado o null si no existe</returns>
    /// <exception cref="BusinessException">Si el ID del hotel es inválido</exception>
    /// <exception cref="DatabaseException">Si ocurre un error en la base de datos</exception>
    Task<Hotel?> GetByIdAsync(int hotelId, string transactionId);

    /// <summary>
    /// Obtiene todos los hoteles registrados en el sistema
    /// </summary>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>Lista de todos los hoteles</returns>
    /// <exception cref="DatabaseException">Si ocurre un error en la base de datos</exception>
    Task<List<Hotel>> GetAllAsync(string transactionId);

    /// <summary>
    /// Obtiene hoteles paginados con búsqueda opcional
    /// </summary>
    /// <param name="pageNumber">Número de página (basado en 1)</param>
    /// <param name="pageSize">Tamaño de la página</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <param name="searchTerm">Término de búsqueda opcional para filtrar por nombre o ciudad</param>
    /// <returns>Objeto de paginación con los hoteles encontrados</returns>
    /// <exception cref="BusinessException">Si los parámetros de paginación son inválidos</exception>
    /// <exception cref="DatabaseException">Si ocurre un error en la base de datos</exception>
    Task<Pagination<Hotel>> GetPaginatedAsync(int pageNumber, int pageSize, string transactionId, string? searchTerm = null);

    /// <summary>
    /// Crea un nuevo hotel en el sistema
    /// </summary>
    /// <param name="hotel">Datos del hotel a crear</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>El hotel creado con su ID asignado</returns>
    /// <exception cref="BusinessException">Si hay violación de reglas de negocio</exception>
    /// <exception cref="DatabaseException">Si hay error en repositorio</exception>
    Task<Hotel> CreateAsync(Hotel hotel, string transactionId);

    /// <summary>
    /// Actualiza un hotel existente
    /// </summary>
    /// <param name="hotel">Datos del hotel a actualizar (debe incluir ID)</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>El hotel actualizado</returns>
    /// <exception cref="BusinessException">Si hay violación de reglas de negocio</exception>
    /// <exception cref="NotFoundException">Si no existe el hotel</exception>
    /// <exception cref="DatabaseException">Si hay error en repositorio</exception>
    Task<Hotel> UpdateAsync(Hotel hotel, string transactionId);

    /// <summary>
    /// Cambia el estado de un hotel (activo/inactivo)
    /// </summary>
    /// <param name="hotelId">Identificador único del hotel</param>
    /// <param name="activo">Nuevo estado del hotel (true = activo, false = inactivo)</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <exception cref="BusinessException">Si el ID del hotel es inválido</exception>
    /// <exception cref="NotFoundException">Si no existe el hotel</exception>
    /// <exception cref="DatabaseException">Si hay error en repositorio</exception>
    Task SetEstadoAsync(int hotelId, bool activo, string transactionId);
}
