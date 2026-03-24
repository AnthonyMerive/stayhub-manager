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
/// Implementación del servicio de habitaciones
/// </summary>
public class HabitacionService(
    IHabitacionRepository habitacionRepository,
    IHotelRepository hotelRepository,
    ITraceability traceability) : IHabitacionService
{
    /// <summary>
    /// Obtiene una habitación por su identificador único
    /// </summary>
    /// <param name="habitacionId">Identificador único de la habitación</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>La habitación encontrada o null si no existe</returns>
    /// <exception cref="BusinessException">Si el ID de la habitación es inválido</exception>
    /// <exception cref="DatabaseException">Si ocurre un error en la base de datos</exception>
    public async Task<Habitacion?> GetByIdAsync(int habitacionId, string transactionId)
    {
        var operation = nameof(GetByIdAsync);
        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                HabitacionOperationMessages.IniciandoObtencionPorId,
                additionalProperties: new Dictionary<string, object>
                {
                       { HabitacionDictionaryKeys.HabitacionId, habitacionId }
                });

            // Validar parámetros de entrada
            if (habitacionId <= 0)
                throw new BusinessException(HabitacionErrorCodes.InvalidHabitacionId, HabitacionErrorMessages.InvalidHabitacionId);

            var result = await habitacionRepository.GetByIdAsync(habitacionId);

            if (!result.Success)
                throw new DatabaseException(string.Format(HabitacionErrorMessageTemplates.ErrorRepositorio, result.Message));


            await traceability.TraceOutAsync(
                transactionId,
                operation,
                HabitacionOperationMessages.FinalizandoObtencionPorId,
                additionalProperties: new Dictionary<string, object>
                {
                    { HabitacionDictionaryKeys.HabitacionId, habitacionId },
                    { HabitacionDictionaryKeys.Found, result.Data != null }
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
                      { HabitacionDictionaryKeys.HabitacionId, habitacionId },
                      { HabitacionDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }

    /// <summary>
    /// Obtiene todas las habitaciones de un hotel específico
    /// </summary>
    /// <param name="hotelId">Identificador único del hotel</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>Lista de habitaciones del hotel</returns>
    /// <exception cref="BusinessException">Si el ID del hotel es inválido</exception>
    /// <exception cref="DatabaseException">Si ocurre un error en la base de datos</exception>
    public async Task<List<Habitacion>> GetByHotelIdAsync(int hotelId, string transactionId)
    {
        var operation = nameof(GetByHotelIdAsync);
        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                HabitacionOperationMessages.IniciandoObtencionPorHotelId,
                additionalProperties: new Dictionary<string, object>
                {
                    { HabitacionDictionaryKeys.HotelId, hotelId }
                });

            // Validar parámetros de entrada
            if (hotelId <= 0)
                throw new BusinessException(HabitacionErrorCodes.InvalidHotelId, HabitacionErrorMessages.InvalidHotelId);

            var result = await habitacionRepository.GetByHotelIdAsync(hotelId);

            if (!result.Success)
                throw new DatabaseException(string.Format(HabitacionErrorMessageTemplates.ErrorRepositorio, result.Message));

            await traceability.TraceOutAsync(
                transactionId,
                operation,
                HabitacionOperationMessages.FinalizandoObtencionPorHotelId,
                additionalProperties: new Dictionary<string, object>
                {
                    { HabitacionDictionaryKeys.HotelId, hotelId },
                    { HabitacionDictionaryKeys.Count, result!.Data!.Count }
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
                        { HabitacionDictionaryKeys.HotelId, hotelId },
                        { HabitacionDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }

    /// <summary>
    /// Obtiene habitaciones paginadas con filtro opcional por hotel
    /// </summary>
    /// <param name="pageNumber">Número de página (basado en 1)</param>
    /// <param name="pageSize">Tamaño de la página</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <param name="hotelId">Identificador opcional del hotel para filtrar</param>
    /// <returns>Objeto de paginación con las habitaciones encontradas</returns>
    /// <exception cref="BusinessException">Si los parámetros de paginación o el ID del hotel son inválidos</exception>
    /// <exception cref="DatabaseException">Si ocurre un error en la base de datos</exception>
    public async Task<Pagination<Habitacion>> GetPaginatedAsync(int pageNumber, int pageSize, string transactionId, int? hotelId = null)
    {
        var operation = nameof(GetPaginatedAsync);
        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                HabitacionOperationMessages.IniciandoObtencionPaginada,
                additionalProperties: new Dictionary<string, object>
                {
                    { HabitacionDictionaryKeys.PageNumber, pageNumber },
                    { HabitacionDictionaryKeys.PageSize, pageSize },
                    { HabitacionDictionaryKeys.HotelId, hotelId?.ToString() ?? HabitacionDictionaryKeys.Null }
                });

            // Validar parámetros de entrada
            HabitacionValidations.ValidatePaginationParameters(pageNumber, pageSize);

            if (hotelId.HasValue && hotelId.Value <= 0)
                throw new BusinessException(HabitacionErrorCodes.InvalidHotelId, HabitacionErrorMessages.InvalidHotelId);

            var result = await habitacionRepository.GetPaginatedAsync(pageNumber, pageSize, hotelId);

            if (!result.Success)
                throw new DatabaseException(string.Format(HabitacionErrorMessageTemplates.ErrorRepositorio, result.Message));

            await traceability.TraceOutAsync(
                transactionId,
                operation,
                HabitacionOperationMessages.FinalizandoObtencionPaginada,
                additionalProperties: new Dictionary<string, object>
                {
                    { HabitacionDictionaryKeys.PageNumber, pageNumber },
                    { HabitacionDictionaryKeys.PageSize, pageSize },
                    { HabitacionDictionaryKeys.HotelId, hotelId?.ToString() ?? HabitacionDictionaryKeys.Null },
                    { HabitacionDictionaryKeys.TotalRecords, result!.Data!.TotalRecords },
                    { HabitacionDictionaryKeys.TotalPages, result.Data.TotalPages }
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
                        { HabitacionDictionaryKeys.PageNumber, pageNumber },
                        { HabitacionDictionaryKeys.PageSize, pageSize },
                        { HabitacionDictionaryKeys.HotelId, hotelId?.ToString() ?? HabitacionDictionaryKeys.Null },
                        { HabitacionDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }

    /// <summary>
    /// Crea una nueva habitación en el sistema
    /// </summary>
    /// <param name="habitacion">Datos de la habitación a crear</param>
    /// <param name="transactionId">Identificador de transacción para trazabilidad</param>
    /// <returns>La habitación creada con su ID asignado</returns>
    /// <exception cref="BusinessException">Si hay violación de reglas de negocio</exception>
    /// <exception cref="DatabaseException">Si hay error en repositorio</exception>
    public async Task<Habitacion> CreateAsync(Habitacion habitacion, string transactionId)
    {
        var operation = nameof(CreateAsync);
        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                HabitacionOperationMessages.IniciandoCreacionHabitacion,
                additionalProperties: new Dictionary<string, object>
                {
                    { HabitacionDictionaryKeys.NumeroHabitacion, habitacion.NumeroHabitacion },
                    { HabitacionDictionaryKeys.HotelId, habitacion.HotelId },
                    { HabitacionDictionaryKeys.TipoHabitacion, habitacion.TipoHabitacion },
                    { HabitacionDictionaryKeys.Capacidad, habitacion.Capacidad },
                    { HabitacionDictionaryKeys.TarifaNoche, habitacion.TarifaNoche }
                });

            // Sanitizar y validar datos de entrada
            HabitacionValidations.SanitizeHabitacionData(habitacion, isUpdate: false);

            // Validar que el hotel exista
            var hotelExistsResult = await hotelRepository.ExistsAsync(habitacion.HotelId);

            if (!hotelExistsResult.Success)
                throw new DatabaseException(string.Format(HabitacionErrorMessageTemplates.ErrorVerificandoExistenciaHotel, hotelExistsResult.Message));

            if (!hotelExistsResult.Data)
                throw new NotFoundException(HabitacionEntityNames.Hotel, habitacion.HotelId);

            var result = await habitacionRepository.CreateAsync(habitacion);

            if (!result.Success)
                throw new DatabaseException(string.Format(HabitacionErrorMessageTemplates.ErrorRepositorio, result.Message));

            await traceability.TraceInAsync(transactionId, operation,
                HabitacionOperationMessages.HabitacionCreadaExitosamente,
                additionalProperties: new Dictionary<string, object>
                {
                    { HabitacionDictionaryKeys.HabitacionId, result.Data!.HabitacionId },
                    { HabitacionDictionaryKeys.NumeroHabitacion, result.Data.NumeroHabitacion }
                });

            await traceability.TraceOutAsync(
                transactionId,
                operation,
                HabitacionOperationMessages.FinalizandoCreacionHabitacion,
                additionalProperties: new Dictionary<string, object>
                {
                    { HabitacionDictionaryKeys.HabitacionId, result.Data!.HabitacionId },
                    { HabitacionDictionaryKeys.NumeroHabitacion, result.Data.NumeroHabitacion },
                    { HabitacionDictionaryKeys.Success, true }
                });

            return result.Data!;
        }
        catch (Exception ex)
        {
            await traceability.TraceErrorAsync(
                transactionId,
                operation,
                ex,
                additionalProperties: new Dictionary<string, object>
                {
                        { HabitacionDictionaryKeys.NumeroHabitacion, habitacion?.NumeroHabitacion ?? HabitacionDictionaryKeys.Null },
                        { HabitacionDictionaryKeys.HotelId, habitacion?.HotelId ?? 0 },
                        { HabitacionDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }

    public async Task<Habitacion> UpdateAsync(Habitacion habitacion, string transactionId)
    {
        var operation = nameof(UpdateAsync);
        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                HabitacionOperationMessages.IniciandoActualizacionHabitacion,
                additionalProperties: new Dictionary<string, object>
                {
                    { HabitacionDictionaryKeys.HabitacionId, habitacion.HabitacionId },
                    { HabitacionDictionaryKeys.NumeroHabitacion, habitacion.NumeroHabitacion }
                });

            // Sanitizar y validar datos de entrada
            HabitacionValidations.SanitizeHabitacionData(habitacion, isUpdate: true);

            var existsResult = await habitacionRepository.ExistsAsync(habitacion.HabitacionId);

            if (!existsResult.Success)
                throw new DatabaseException(string.Format(HabitacionErrorMessageTemplates.ErrorVerificandoExistencia, existsResult.Message));

            if (!existsResult.Data)
                throw new NotFoundException(HabitacionEntityNames.Habitacion, habitacion.HabitacionId);

            var result = await habitacionRepository.UpdateAsync(habitacion);

            if (!result.Success)
            {
                if (result.ErrorCode == HabitacionErrorCodes.NotFound)
                {
                    throw new NotFoundException(HabitacionEntityNames.Habitacion, habitacion.HabitacionId);
                }

                throw new DatabaseException(string.Format(HabitacionErrorMessageTemplates.ErrorRepositorio, result.Message));
            }

            await traceability.TraceInAsync(transactionId, operation,
                HabitacionOperationMessages.HabitacionActualizadaExitosamente,
                additionalProperties: new Dictionary<string, object>
                {
                    { HabitacionDictionaryKeys.HabitacionId, habitacion.HabitacionId }
                });

            await traceability.TraceOutAsync(
                transactionId,
                operation,
                HabitacionOperationMessages.FinalizandoActualizacionHabitacion,
                additionalProperties: new Dictionary<string, object>
                {
                    { HabitacionDictionaryKeys.HabitacionId, habitacion.HabitacionId },
                    { HabitacionDictionaryKeys.NumeroHabitacion, result.Data!.NumeroHabitacion },
                    { HabitacionDictionaryKeys.Success, true }
                });

            return result.Data!;
        }
        catch (Exception ex)
        {
            await traceability.TraceErrorAsync(
                transactionId,
                operation,
                ex,
                additionalProperties: new Dictionary<string, object>
                {
                        { HabitacionDictionaryKeys.HabitacionId, habitacion?.HabitacionId ?? 0 },
                        { HabitacionDictionaryKeys.NumeroHabitacion, habitacion?.NumeroHabitacion ?? HabitacionDictionaryKeys.Null },
                        { HabitacionDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }

    public async Task SetEstadoAsync(int habitacionId, bool activo, string transactionId)
    {
        var operation = nameof(SetEstadoAsync);

        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                HabitacionOperationMessages.IniciandoCambioEstadoHabitacion,
                additionalProperties: new Dictionary<string, object>
                {
                    { HabitacionDictionaryKeys.HabitacionId, habitacionId },
                    { HabitacionDictionaryKeys.NuevoEstado, activo ? HabitacionDictionaryKeys.Activo : HabitacionDictionaryKeys.Inactivo }
                });

            // Validar parámetros de entrada
            if (habitacionId <= 0)
                throw new BusinessException(HabitacionErrorCodes.InvalidHabitacionId, HabitacionErrorMessages.InvalidHabitacionId);

            var result = await habitacionRepository.SetEstadoAsync(habitacionId, activo);

            if (!result.Success)
            {
                if (result.ErrorCode == HabitacionErrorCodes.NotFound)
                {
                    throw new NotFoundException(HabitacionEntityNames.Habitacion, habitacionId);
                }

                throw new DatabaseException(string.Format(HabitacionErrorMessageTemplates.ErrorRepositorio, result.Message));
            }

            await traceability.TraceOutAsync(
                transactionId,
                operation,
                HabitacionOperationMessages.FinalizandoCambioEstadoHabitacion,
                additionalProperties: new Dictionary<string, object>
                {
                    { HabitacionDictionaryKeys.HabitacionId, habitacionId },
                    { HabitacionDictionaryKeys.NuevoEstado, activo ? HabitacionDictionaryKeys.Activo : HabitacionDictionaryKeys.Inactivo },
                    { HabitacionDictionaryKeys.Success, true }
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
                        { HabitacionDictionaryKeys.HabitacionId, habitacionId },
                        { HabitacionDictionaryKeys.NuevoEstado, activo ? HabitacionDictionaryKeys.Activo : HabitacionDictionaryKeys.Inactivo },
                        { HabitacionDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }

    /// <summary>
    /// BR-04: Solo habitaciones activas con capacidad suficiente y sin overbooking
    /// </summary>
    public async Task<List<Habitacion>> GetDisponiblesAsync(int hotelId, DateTime fechaEntrada, DateTime fechaSalida, int cantidadHuespedes, string transactionId)
    {
        var operation = nameof(GetDisponiblesAsync);
        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                HabitacionOperationMessages.IniciandoBusquedaHabitacionesDisponibles,
                additionalProperties: new Dictionary<string, object>
                {
                    { HabitacionDictionaryKeys.HotelId, hotelId },
                    { HabitacionDictionaryKeys.FechaEntrada, fechaEntrada },
                    { HabitacionDictionaryKeys.FechaSalida, fechaSalida },
                    { HabitacionDictionaryKeys.CantidadHuespedes, cantidadHuespedes }
                });

            // Validar parámetros de entrada
            HabitacionValidations.ValidateAvailabilityParameters(hotelId, fechaEntrada, fechaSalida, cantidadHuespedes);

            var result = await habitacionRepository.GetDisponiblesAsync(hotelId, fechaEntrada, fechaSalida, cantidadHuespedes);

            if (!result.Success)
                throw new DatabaseException(string.Format(HabitacionErrorMessageTemplates.ErrorRepositorio, result.Message));

            await traceability.TraceOutAsync(
                transactionId,
                operation,
                HabitacionOperationMessages.FinalizandoBusquedaHabitacionesDisponibles,
                additionalProperties: new Dictionary<string, object>
                {
                    { HabitacionDictionaryKeys.HotelId, hotelId },
                    { HabitacionDictionaryKeys.FechaEntrada, fechaEntrada },
                    { HabitacionDictionaryKeys.FechaSalida, fechaSalida },
                    { HabitacionDictionaryKeys.CantidadHuespedes, cantidadHuespedes },
                    { HabitacionDictionaryKeys.HabitacionesDisponibles, result!.Data!.Count }
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
                        { HabitacionDictionaryKeys.HotelId, hotelId },
                        { HabitacionDictionaryKeys.FechaEntrada, fechaEntrada },
                        { HabitacionDictionaryKeys.FechaSalida, fechaSalida },
                        { HabitacionDictionaryKeys.CantidadHuespedes, cantidadHuespedes },
                        { HabitacionDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }
}
