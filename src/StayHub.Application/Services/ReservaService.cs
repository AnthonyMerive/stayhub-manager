using StayHub.Application.Ports.In.Services;
using StayHub.Application.Ports.Out.Database;
using StayHub.Application.Ports.Out.Traceability;
using StayHub.Application.Rules;
using StayHub.Domain.Constants.Reserva;
using StayHub.Domain.Entities;
using StayHub.Domain.Enums;
using StayHub.Domain.Exceptions;
using StayHub.Shared.Types;

namespace StayHub.Application.Services;

/// <summary>
/// Implementación del servicio de reservas
/// Aplica las reglas de negocio BR-01 a BR-06
/// </summary>
public class ReservaService(
    IReservaRepository reservaRepository,
    IHabitacionRepository habitacionRepository,
    ITraceability traceability) : IReservaService
{

    public async Task<Reserva?> GetByIdAsync(int reservaId, string transactionId)
    {
        const string operation = nameof(GetByIdAsync);

        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                ReservaOperationMessages.IniciandoObtencionPorId,
                new Dictionary<string, object> {
                    { ReservaDictionaryKeys.ReservaId, reservaId }
                });

            // Validar parámetros de entrada
            if (reservaId <= 0)
                throw new BusinessException(ReservaErrorCodes.InvalidReservaId, ReservaErrorMessages.InvalidReservaId);

            var result = await reservaRepository.GetByIdAsync(reservaId);

            if (!result.Success) 
                throw new DatabaseException(string.Format(ReservaErrorMessageTemplates.ErrorObtenerReserva, result.Message));

            await traceability.TraceOutAsync(
                transactionId,
                operation,
                ReservaOperationMessages.FinalizandoObtencionPorId,
                additionalProperties: new Dictionary<string, object>
                {
                    { ReservaDictionaryKeys.ReservaId, reservaId },
                    { ReservaDictionaryKeys.Found, result.Data != null }
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
                    { ReservaDictionaryKeys.ReservaId, reservaId },
                    { ReservaDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }

    public async Task<List<Reserva>> GetByHotelIdAsync(int hotelId, string transactionId)
    {
        const string operation = nameof(GetByHotelIdAsync);

        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                ReservaOperationMessages.IniciandoObtencionPorHotelId,
                additionalProperties: new Dictionary<string, object>
                {
                    { ReservaDictionaryKeys.HotelId, hotelId }
                });

            // Validar parámetros de entrada
            if (hotelId <= 0)
            {
                throw new BusinessException(ReservaErrorCodes.InvalidHotelId, ReservaErrorMessages.InvalidHotelId);
            }

            var result = await reservaRepository.GetByHotelIdAsync(hotelId);

            if (!result.Success)
                throw new DatabaseException(string.Format(ReservaErrorMessageTemplates.ErrorObtenerReservasHotel, result.Message));


            await traceability.TraceOutAsync(
                transactionId,
                operation,
                ReservaOperationMessages.FinalizandoObtencionPorHotelId,
                additionalProperties: new Dictionary<string, object>
                {
                    { ReservaDictionaryKeys.HotelId, hotelId },
                    { ReservaDictionaryKeys.Count, result!.Data!.Count }
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
                    { ReservaDictionaryKeys.HotelId, hotelId },
                    { ReservaDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }

    public async Task<Pagination<Reserva>> GetPaginatedAsync(int pageNumber, int pageSize, string transactionId, int? hotelId = null, int? habitacionId = null)
    {
        const string operation = nameof(GetPaginatedAsync);

        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                ReservaOperationMessages.IniciandoObtencionPaginada,
                additionalProperties: new Dictionary<string, object>
                {
                    { ReservaDictionaryKeys.PageNumber, pageNumber },
                    { ReservaDictionaryKeys.PageSize, pageSize },
                    { ReservaDictionaryKeys.HotelId, hotelId?.ToString() ?? ReservaDictionaryKeys.Null },
                    { ReservaDictionaryKeys.HabitacionId, habitacionId?.ToString() ?? ReservaDictionaryKeys.Null }
                });

            ReservaValidations.ValidatePaginationParameters(pageNumber, pageSize);

            if (hotelId.HasValue && hotelId.Value <= 0)
                throw new BusinessException(ReservaErrorCodes.InvalidHotelId, ReservaErrorMessages.InvalidHotelId);

            if (habitacionId.HasValue && habitacionId.Value <= 0)
                throw new BusinessException(ReservaErrorCodes.InvalidHabitacionId, ReservaErrorMessages.InvalidHabitacionId);

            var result = await reservaRepository.GetPaginatedAsync(pageNumber, pageSize, hotelId, habitacionId);

            if (!result.Success)
                throw new DatabaseException(string.Format(ReservaErrorMessageTemplates.ErrorObtenerReservasPaginadas, result.Message));


            await traceability.TraceOutAsync(
                transactionId,
                operation,
                ReservaOperationMessages.FinalizandoObtencionPaginada,
                additionalProperties: new Dictionary<string, object>
                {
                    { ReservaDictionaryKeys.PageNumber, pageNumber },
                    { ReservaDictionaryKeys.PageSize, pageSize },
                    { ReservaDictionaryKeys.TotalRecords, result!.Data!.TotalRecords },
                    { ReservaDictionaryKeys.TotalPages, result.Data.TotalPages },
                    { ReservaDictionaryKeys.CurrentPageCount, result.Data.Items.Count }
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
                    { ReservaDictionaryKeys.PageNumber, pageNumber },
                    { ReservaDictionaryKeys.PageSize, pageSize },
                    { ReservaDictionaryKeys.HotelId, hotelId?.ToString() ?? ReservaDictionaryKeys.Null },
                    { ReservaDictionaryKeys.HabitacionId, habitacionId?.ToString() ?? ReservaDictionaryKeys.Null },
                    { ReservaDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }

    public async Task<Reserva> CreateAsync(Reserva reserva, string transactionId)
    {
        const string operation = nameof(CreateAsync);

        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                ReservaOperationMessages.IniciandoCreacionReserva,
                additionalProperties: new Dictionary<string, object>
                {
                    { ReservaDictionaryKeys.HotelId, reserva.HotelId },
                    { ReservaDictionaryKeys.HabitacionId, reserva.HabitacionId },
                    { ReservaDictionaryKeys.FechaEntrada, reserva.FechaEntrada },
                    { ReservaDictionaryKeys.FechaSalida, reserva.FechaSalida },
                    { ReservaDictionaryKeys.CantidadHuespedes, reserva.CantidadHuespedes }
                });

            // Sanitizar y validar datos de entrada
            ReservaValidations.SanitizeReservaData(reserva, isUpdate: false);

            // BR-04: Validar que la habitación esté activa
            var habitacionResult = await habitacionRepository.GetByIdAsync(reserva.HabitacionId);

            if (!habitacionResult.Success)
                throw new DatabaseException(string.Format(ReservaErrorMessageTemplates.ErrorVerificarHabitacion, habitacionResult.Message));

            if (habitacionResult.Data == null)
                throw new NotFoundException(ReservaEntityNames.Habitacion, reserva.HabitacionId);

            if (habitacionResult.Data.Estado != Estado.Activo)
                throw new BusinessException(ReservaErrorCodes.HabitacionInactiva, ReservaErrorMessages.HabitacionInactiva);

            // BR-03: Validar capacidad de la habitación
            if (reserva.CantidadHuespedes > habitacionResult.Data.Capacidad)
                throw new BusinessException(ReservaErrorCodes.CapacidadExcedida, string.Format(ReservaErrorMessageTemplates.CapacidadExcedida, reserva.CantidadHuespedes, habitacionResult.Data.Capacidad));

            var overbookingResult = await reservaRepository.ExisteOverbookingAsync(reserva.HabitacionId, reserva.FechaEntrada, reserva.FechaSalida);

            // BR-02: Prevención de overbooking
            if (!overbookingResult.Success)
            {
                throw new DatabaseException(string.Format(ReservaErrorMessageTemplates.ErrorValidarOverbooking, overbookingResult.Message));
            }

            if (overbookingResult.Data)
            {
                throw new BusinessException(ReservaErrorCodes.OverbookingDetected, ReservaErrorMessages.OverbookingDetected);
            }

            // BR-05: Cálculo automático del total
            var numberOfNights = (reserva.FechaSalida - reserva.FechaEntrada).Days;
            reserva.ValorNoche = habitacionResult.Data.TarifaNoche;
            reserva.TotalReserva = reserva.ValorNoche * numberOfNights;

            // Asignar estado inicial
            reserva.EstadoReserva = EstadoReserva.Activa;

            var result = await reservaRepository.CreateAsync(reserva);

            if (!result.Success)
                throw new DatabaseException(string.Format(ReservaErrorMessageTemplates.ErrorCrearReserva, result.Message));


            await traceability.TraceOutAsync(
                transactionId,
                operation,
                ReservaOperationMessages.FinalizandoCreacionReserva,
                additionalProperties: new Dictionary<string, object>
                {
                    { ReservaDictionaryKeys.ReservaId, result!.Data!.ReservaId },
                    { ReservaDictionaryKeys.TotalReserva, result.Data.TotalReserva },
                    { ReservaDictionaryKeys.EstadoReserva, result.Data.EstadoReserva }
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
                    { ReservaDictionaryKeys.HotelId, reserva?.HotelId ?? 0 },
                    { ReservaDictionaryKeys.HabitacionId, reserva?.HabitacionId ?? 0 },
                    { ReservaDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }

    public async Task CancelarAsync(int reservaId, string transactionId)
    {
        const string operation = nameof(CancelarAsync);

        try
        {
            await traceability.TraceInAsync(
                transactionId,
                operation,
                ReservaOperationMessages.IniciandoCancelacionReserva,
                additionalProperties: new Dictionary<string, object>
                {
                    { ReservaDictionaryKeys.ReservaId, reservaId }
                });

            // Validar parámetros de entrada
            if (reservaId <= 0)
                throw new BusinessException(ReservaErrorCodes.InvalidReservaId, ReservaErrorMessages.InvalidReservaId);

            // Verificar que la reserva existe
            var existingResult = await reservaRepository.GetByIdAsync(reservaId);

            if (!existingResult.Success)
                throw new DatabaseException(string.Format(ReservaErrorMessageTemplates.ErrorVerificarReservaExistente, existingResult.Message));

            if (existingResult.Data == null)
                throw new NotFoundException(ReservaEntityNames.Reserva, reservaId);

            // BR-06: Validar que la reserva no esté ya cancelada
            if (existingResult.Data.EstadoReserva == EstadoReserva.Cancelada)
                throw new BusinessException(ReservaErrorCodes.ReservaYaCancelada, ReservaErrorMessages.ReservaYaCancelada);

            var result = await reservaRepository.CancelarAsync(reservaId);

            if (!result.Success)
                throw new DatabaseException(string.Format(ReservaErrorMessageTemplates.ErrorCancelarReserva, result.Message));

            await traceability.TraceOutAsync(
                transactionId,
                operation,
                ReservaOperationMessages.FinalizandoCancelacionReserva,
                additionalProperties: new Dictionary<string, object>
                {
                    { ReservaDictionaryKeys.ReservaId, reservaId }
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
                    { ReservaDictionaryKeys.ReservaId, reservaId },
                    { ReservaDictionaryKeys.ExceptionType, ex.GetType().Name }
                });
            throw;
        }
    }
}