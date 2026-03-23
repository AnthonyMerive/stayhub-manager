using StayHub.Domain.Entities;
using StayHub.Domain.Exceptions;

namespace StayHub.Application.Rules
{
    public static class HabitacionValidations
    {
        /// <summary>
        /// Sanitiza y valida los datos de entrada de una habitación
        /// </summary>
        public static void SanitizeHabitacionData(Habitacion habitacion, bool isUpdate = false)
        {
            // Validar que el objeto no sea null
            ArgumentNullException.ThrowIfNull(habitacion);

            // Validar HabitacionId para actualizaciones
            if (isUpdate && habitacion.HabitacionId <= 0)
            {
                throw new BusinessException("INVALID_HABITACION_ID",
                    "El ID de la habitación debe ser un valor positivo.");
            }

            // Validar HotelId
            if (habitacion.HotelId <= 0)
            {
                throw new BusinessException("INVALID_HOTEL_ID",
                    "El ID del hotel debe ser un valor positivo.");
            }

            // Sanitizar y validar NumeroHabitacion
            habitacion.NumeroHabitacion = SanitizeString(habitacion.NumeroHabitacion);
            if (string.IsNullOrWhiteSpace(habitacion.NumeroHabitacion))
            {
                throw new BusinessException("NUMERO_HABITACION_REQUIRED",
                    "El número de habitación es obligatorio.");
            }

            // Remover espacios extra del número de habitación
            habitacion.NumeroHabitacion = habitacion.NumeroHabitacion.Trim();

            // Validar longitud del número de habitación
            if (habitacion.NumeroHabitacion.Length > 10)
            {
                throw new BusinessException("NUMERO_HABITACION_TOO_LONG",
                    "El número de habitación no puede exceder los 10 caracteres.");
            }

            // Sanitizar y validar TipoHabitacion
            habitacion.TipoHabitacion = SanitizeString(habitacion.TipoHabitacion);
            if (string.IsNullOrWhiteSpace(habitacion.TipoHabitacion))
            {
                throw new BusinessException("TIPO_HABITACION_REQUIRED",
                    "El tipo de habitación es obligatorio.");
            }

            // Remover espacios extra y normalizar el tipo de habitación
            habitacion.TipoHabitacion = habitacion.TipoHabitacion.Trim();

            // Validar longitud del tipo de habitación
            if (habitacion.TipoHabitacion.Length > 50)
            {
                throw new BusinessException("TIPO_HABITACION_TOO_LONG",
                    "El tipo de habitación no puede exceder los 50 caracteres.");
            }

            // Validar Capacidad
            if (habitacion.Capacidad <= 0)
            {
                throw new BusinessException("INVALID_CAPACIDAD",
                    "La capacidad debe ser un valor positivo.");
            }

            if (habitacion.Capacidad > 20)
            {
                throw new BusinessException("CAPACIDAD_TOO_HIGH",
                    "La capacidad no puede ser mayor a 20 huéspedes.");
            }

            // Validar TarifaNoche
            if (habitacion.TarifaNoche <= 0)
            {
                throw new BusinessException("INVALID_TARIFA",
                    "La tarifa por noche debe ser un valor positivo.");
            }

            if (habitacion.TarifaNoche > 999999.99m)
            {
                throw new BusinessException("TARIFA_TOO_HIGH",
                    "La tarifa por noche no puede exceder $999,999.99.");
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
        /// Valida parámetros de búsqueda de disponibilidad
        /// </summary>
        public static void ValidateAvailabilityParameters(int hotelId, DateTime fechaEntrada, DateTime fechaSalida, int cantidadHuespedes)
        {
            if (hotelId <= 0)
            {
                throw new BusinessException("INVALID_HOTEL_ID",
                    "El ID del hotel debe ser un valor positivo.");
            }

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

            if (cantidadHuespedes <= 0)
            {
                throw new BusinessException("INVALID_GUEST_COUNT",
                    "La cantidad de huéspedes debe ser un valor positivo.");
            }

            if (cantidadHuespedes > 20)
            {
                throw new BusinessException("TOO_MANY_GUESTS",
                    "La cantidad de huéspedes no puede exceder 20 personas.");
            }
        }
    }
}
