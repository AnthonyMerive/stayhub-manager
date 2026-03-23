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