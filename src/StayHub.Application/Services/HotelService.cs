using StayHub.Application.Ports.In.Services;
using StayHub.Application.Ports.Out.Database;
using StayHub.Application.Ports.Out.Traceability;
using StayHub.Application.Rules;
using StayHub.Application.Rules.Constants;
using StayHub.Domain.Entities;
using StayHub.Domain.Exceptions;
using StayHub.Shared.Types;

namespace StayHub.Application.Services;

/// <summary>
/// Implementación del servicio de hoteles
/// </summary>
public class HotelService(
    IHotelRepository hotelRepository,
    ITraceability traceability) : IHotelService
{
    /// <summary>
    /// Obtiene un hotel por su identificador único
    /// </summary>
    /// <param name="hotelId">Identificador único del hotel</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>El hotel encontrado o null si no existe</returns>
    /// <exception cref="BusinessException">Si el ID del hotel es inválido</exception>
    /// <exception cref="DatabaseException">Si ocurre un error en la base de datos</exception>
    public async Task<Hotel?> GetByIdAsync(int hotelId, string transactionId)
    {
        const string operation = nameof(GetByIdAsync);

        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                HotelOperationMessages.IniciandoObtencionPorId,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.HotelId, hotelId }
                });

            // Validar parámetros de entrada
            if (hotelId <= 0)
                throw new BusinessException(HotelErrorCodes.InvalidHotelId, HotelErrorMessages.InvalidHotelId);

            var result = await hotelRepository.GetByIdAsync(hotelId);

            if (!result.Success)
                throw new DatabaseException(string.Format(HotelErrorMessageTemplates.ErrorObtenerHotel, result.Message));

            await traceability.TraceOutAsync(
                transactionId,
                operation,
                HotelOperationMessages.FinalizandoObtencionPorId,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.HotelId, hotelId },
                    { HotelDictionaryKeys.Found, result.Data != null }
                });

            return result.Data;
        }
        catch (Exception ex) 
        {
            await traceability.TraceErrorAsync(
                transactionId,
                operation,
                ex,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.HotelId, hotelId },
                    { HotelDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los hoteles registrados en el sistema
    /// </summary>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>Lista de todos los hoteles</returns>
    /// <exception cref="DatabaseException">Si ocurre un error en la base de datos</exception>
    public async Task<List<Hotel>> GetAllAsync(string transactionId)
    {
        const string operation = nameof(GetAllAsync);

        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                HotelOperationMessages.IniciandoObtencionTodos);

            var result = await hotelRepository.GetAllAsync();

            if (!result.Success)
                throw new DatabaseException(string.Format(HotelErrorMessageTemplates.ErrorObtenerHoteles, result.Message));

            await traceability.TraceOutAsync(
                transactionId,
                operation,
                HotelOperationMessages.FinalizandoObtencionTodos,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.Count, result!.Data!.Count }
                });

            return result.Data;
        }
        catch (Exception ex) 
        {
            await traceability.TraceErrorAsync(
                transactionId,
                operation,
                ex,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }

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
    public async Task<Pagination<Hotel>> GetPaginatedAsync(int pageNumber, int pageSize, string transactionId, string? searchTerm = null)
    {
        const string operation = nameof(GetPaginatedAsync);

        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                HotelOperationMessages.IniciandoObtencionPaginada,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.PageNumber, pageNumber },
                    { HotelDictionaryKeys.PageSize, pageSize },
                    { HotelDictionaryKeys.SearchTerm, searchTerm ?? HotelDictionaryKeys.Null }
                });

            // Validar parámetros de entrada
            HotelValidations.ValidatePaginationParameters(pageNumber, pageSize);

            // Sanitizar término de búsqueda
            var sanitizedSearchTerm = HotelValidations.SanitizeString(searchTerm);

            var result = await hotelRepository.GetPaginatedAsync(pageNumber, pageSize, sanitizedSearchTerm);

            if (!result.Success)
                throw new DatabaseException(string.Format(HotelErrorMessageTemplates.ErrorObtenerHotelesPaginados, result.Message));

            await traceability.TraceOutAsync(
                transactionId,
                operation,
                HotelOperationMessages.FinalizandoObtencionPaginada,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.PageNumber, pageNumber },
                    { HotelDictionaryKeys.PageSize, pageSize },
                    { HotelDictionaryKeys.TotalRecords, result!.Data!.TotalRecords },
                    { HotelDictionaryKeys.TotalPages, result.Data.TotalPages },
                    { HotelDictionaryKeys.CurrentPageCount, result.Data.Items.Count }
                });

            return result.Data;
        }
        catch (Exception ex) 
        {
            await traceability.TraceErrorAsync(
                transactionId,
                operation,
                ex,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.PageNumber, pageNumber },
                    { HotelDictionaryKeys.PageSize, pageSize },
                    { HotelDictionaryKeys.SearchTerm, searchTerm ?? HotelDictionaryKeys.Null },
                    { HotelDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }

    /// <summary>
    /// Crea un nuevo hotel en el sistema
    /// </summary>
    /// <param name="hotel">Datos del hotel a crear</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>El hotel creado con su ID asignado</returns>
    /// <exception cref="BusinessException">Si hay violación de reglas de negocio</exception>
    /// <exception cref="DatabaseException">Si hay error en repositorio</exception>
    public async Task<Hotel> CreateAsync(Hotel hotel, string transactionId)
    {
        const string operation = nameof(CreateAsync);

        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                HotelOperationMessages.IniciandoCreacionHotel,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.Nombre, hotel.Nombre },
                    { HotelDictionaryKeys.Ciudad, hotel.Ciudad }
                });

            // Sanitizar y validar datos de entrada
            HotelValidations.SanitizeHotelData(hotel, isUpdate: false);

            var result = await hotelRepository.CreateAsync(hotel);

            if (!result.Success)
                throw new DatabaseException(string.Format(HotelErrorMessageTemplates.ErrorCrearHotel, result.Message));

            await traceability.TraceOutAsync(
                transactionId,
                operation,
                HotelOperationMessages.FinalizandoCreacionHotel,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.HotelId, result!.Data!.HotelId },
                    { HotelDictionaryKeys.Nombre, result.Data.Nombre }
                });

            return result.Data;
        }
        catch (Exception ex) 
        {
            await traceability.TraceErrorAsync(
                transactionId,
                operation,
                ex,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.Nombre, hotel?.Nombre ?? HotelDictionaryKeys.Null },
                    { HotelDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }

    /// <summary>
    /// Actualiza un hotel existente
    /// </summary>
    /// <param name="hotel">Datos del hotel a actualizar (debe incluir ID)</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>El hotel actualizado</returns>
    /// <exception cref="BusinessException">Si hay violación de reglas de negocio</exception>
    /// <exception cref="NotFoundException">Si no existe el hotel</exception>
    /// <exception cref="DatabaseException">Si hay error en repositorio</exception>
    public async Task<Hotel> UpdateAsync(Hotel hotel, string transactionId)
    {
        const string operation = nameof(UpdateAsync);

        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                HotelOperationMessages.IniciandoActualizacionHotel,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.HotelId, hotel.HotelId },
                    { HotelDictionaryKeys.Nombre, hotel.Nombre }
                });

            // Sanitizar y validar datos de entrada
            HotelValidations.SanitizeHotelData(hotel, isUpdate: true);

            // Verificar que el hotel existe
            var existingResult = await hotelRepository.GetByIdAsync(hotel.HotelId);

            if (!existingResult.Success)
                throw new DatabaseException(string.Format(HotelErrorMessageTemplates.ErrorVerificarHotelExistente, existingResult.Message));

            if (existingResult.Data == null)
                throw new NotFoundException(HotelEntityNames.Hotel, hotel.HotelId);

            var result = await hotelRepository.UpdateAsync(hotel);

            if (!result.Success)
                throw new DatabaseException(string.Format(HotelErrorMessageTemplates.ErrorActualizarHotel, result.Message));

            await traceability.TraceOutAsync(
                transactionId,
                operation,
                HotelOperationMessages.FinalizandoActualizacionHotel,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.HotelId, result!.Data!.HotelId },
                    { HotelDictionaryKeys.Nombre, result.Data.Nombre }
                });

            return result.Data;
        }
        catch (Exception ex) 
        {
            await traceability.TraceErrorAsync(
                transactionId,
                operation,
                ex,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.HotelId, hotel?.HotelId ?? 0 },
                    { HotelDictionaryKeys.Nombre, hotel?.Nombre ?? HotelDictionaryKeys.Null },
                    { HotelDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }

    /// <summary>
    /// Cambia el estado de un hotel (activo/inactivo)
    /// </summary>
    /// <param name="hotelId">Identificador único del hotel</param>
    /// <param name="activo">Nuevo estado del hotel (true = activo, false = inactivo)</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <exception cref="BusinessException">Si el ID del hotel es inválido</exception>
    /// <exception cref="NotFoundException">Si no existe el hotel</exception>
    /// <exception cref="DatabaseException">Si hay error en repositorio</exception>
    public async Task SetEstadoAsync(int hotelId, bool activo, string transactionId)
    {
        const string operation = nameof(SetEstadoAsync);

        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                HotelOperationMessages.IniciandoCambioEstadoHotel,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.HotelId, hotelId },
                    { HotelDictionaryKeys.NewStatus, activo }
                });

            // Validar parámetros de entrada
            if (hotelId <= 0)
                throw new BusinessException(HotelErrorCodes.InvalidHotelId, HotelErrorMessages.InvalidHotelId);

            // Verificar que el hotel existe
            var existingResult = await hotelRepository.GetByIdAsync(hotelId);

            if (!existingResult.Success)
                throw new DatabaseException(string.Format(HotelErrorMessageTemplates.ErrorVerificarHotelExistente, existingResult.Message));

            if (existingResult.Data == null)
                throw new NotFoundException(HotelEntityNames.Hotel, hotelId);

            var result = await hotelRepository.SetEstadoAsync(hotelId, activo);

            if (!result.Success)
                throw new DatabaseException(string.Format(HotelErrorMessageTemplates.ErrorCambiarEstadoHotel, result.Message));

            await traceability.TraceOutAsync(
                transactionId,
                operation,
                HotelOperationMessages.FinalizandoCambioEstadoHotel,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.HotelId, hotelId },
                    { HotelDictionaryKeys.NewStatus, activo }
                });
        }
        catch (Exception ex) 
        {
            await traceability.TraceErrorAsync(
                transactionId,
                operation,
                ex,
                additionalProperties: new Dictionary<string, object>
                {
                    { HotelDictionaryKeys.HotelId, hotelId },
                    { HotelDictionaryKeys.NewStatus, activo },
                    { HotelDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }
}
