using StayHub.Domain.Entities;
using StayHub.Domain.Exceptions;

namespace StayHub.Application.Rules
{
    public static class ReservaValidations
    {
        /// <summary>
        /// Sanitiza y valida los datos de entrada de una reserva
        /// </summary>
        public static void SanitizeReservaData(Reserva reserva, bool isUpdate = false)
        {
            // Validar que el objeto no sea null
            ArgumentNullException.ThrowIfNull(reserva);

            // Validar ReservaId para actualizaciones
            if (isUpdate && reserva.ReservaId <= 0)
            {
                throw new BusinessException("INVALID_RESERVA_ID",
                    "El ID de la reserva debe ser un valor positivo.");
            }

            // Validar HotelId
            if (reserva.HotelId <= 0)
            {
                throw new BusinessException("INVALID_HOTEL_ID",
                    "El ID del hotel debe ser un valor positivo.");
            }

            // Validar HabitacionId
            if (reserva.HabitacionId <= 0)
            {
                throw new BusinessException("INVALID_HABITACION_ID",
                    "El ID de la habitación debe ser un valor positivo.");
            }

            // Validar fechas de reserva (BR-01)
            ValidateReservationDates(reserva.FechaEntrada, reserva.FechaSalida);

            // Sanitizar y validar HuespedNombre
            reserva.HuespedNombre = SanitizeString(reserva.HuespedNombre);
            if (string.IsNullOrWhiteSpace(reserva.HuespedNombre))
            {
                throw new BusinessException("NOMBRE_HUESPED_REQUIRED",
                    "El nombre del huésped es obligatorio.");
            }

            // Remover espacios extra del nombre del huésped
            reserva.HuespedNombre = reserva.HuespedNombre.Trim();

            // Validar longitud del nombre del huésped
            if (reserva.HuespedNombre.Length > 100)
            {
                throw new BusinessException("NOMBRE_HUESPED_TOO_LONG",
                    "El nombre del huésped no puede exceder los 100 caracteres.");
            }

            // Sanitizar y validar HuespedDocumento
            reserva.HuespedDocumento = SanitizeString(reserva.HuespedDocumento);
            if (string.IsNullOrWhiteSpace(reserva.HuespedDocumento))
            {
                throw new BusinessException("DOCUMENTO_HUESPED_REQUIRED",
                    "El documento del huésped es obligatorio.");
            }

            // Remover espacios extra del documento del huésped
            reserva.HuespedDocumento = reserva.HuespedDocumento.Trim();

            // Validar longitud del documento del huésped
            if (reserva.HuespedDocumento.Length > 20)
            {
                throw new BusinessException("DOCUMENTO_HUESPED_TOO_LONG",
                    "El documento del huésped no puede exceder los 20 caracteres.");
            }

            // Note: Reserva entity doesn't have phone field

            // Validar CantidadHuespedes (BR-03)
            if (reserva.CantidadHuespedes <= 0)
            {
                throw new BusinessException("INVALID_NUMERO_HUESPEDES",
                    "El número de huéspedes debe ser un valor positivo.");
            }

            if (reserva.CantidadHuespedes > 50)
            {
                throw new BusinessException("NUMERO_HUESPEDES_TOO_HIGH",
                    "El número de huéspedes no puede exceder 50.");
            }

            // Validar ValorNoche
            if (reserva.ValorNoche < 0)
            {
                throw new BusinessException("INVALID_VALOR_NOCHE",
                    "El valor por noche no puede ser negativo.");
            }

            if (reserva.ValorNoche > 999999.99m)
            {
                throw new BusinessException("VALOR_NOCHE_TOO_HIGH",
                    "El valor por noche no puede exceder $999,999.99.");
            }
        }

        /// <summary>
        /// Sanitiza cadenas de texto para prevenir valores null y normalizar espacios
        /// </summary>
        private static string SanitizeString(string? input)
        {
            if (input is null)
            {
                return string.Empty;
            }

            // Remover espacios al inicio y al final
            var sanitized = input.Trim();

            // Normalizar múltiples espacios en blanco a un solo espacio
            while (sanitized.Contains("  "))
            {
                sanitized = sanitized.Replace("  ", " ");
            }

            return sanitized;
        }

        /// <summary>
        /// Valida parámetros de paginación
        /// </summary>
        public static void ValidatePaginationParameters(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
            {
                throw new BusinessException("INVALID_PAGE_NUMBER",
                    "El número de página debe ser un valor positivo.");
            }

            if (pageSize <= 0)
            {
                throw new BusinessException("INVALID_PAGE_SIZE",
                    "El tamaño de página debe ser un valor positivo.");
            }

            if (pageSize > 100)
            {
                throw new BusinessException("PAGE_SIZE_TOO_LARGE",
                    "El tamaño de página no puede ser mayor a 100.");
            }
        }

        /// <summary>
        /// Valida fechas de reserva (BR-01)
        /// </summary>
        private static void ValidateReservationDates(DateTime fechaEntrada, DateTime fechaSalida)
        {
            if (fechaEntrada < DateTime.Today)
            {
                throw new BusinessException("INVALID_CHECK_IN_DATE",
                    "La fecha de entrada no puede ser anterior a hoy.");
            }

            if (fechaSalida <= fechaEntrada)
            {
                throw new BusinessException("INVALID_CHECK_OUT_DATE",
                    "La fecha de salida debe ser posterior a la fecha de entrada.");
            }

            var daysDifference = (fechaSalida - fechaEntrada).Days;
            if (daysDifference > 365)
            {
                throw new BusinessException("STAY_TOO_LONG",
                    "La estadía no puede exceder los 365 días.");
            }

            // Validar que no sea más de 2 años en el futuro
            if (fechaEntrada > DateTime.Today.AddYears(2))
            {
                throw new BusinessException("CHECK_IN_TOO_FAR",
                    "La fecha de entrada no puede ser más de 2 años en el futuro.");
            }
        }
    }
}
