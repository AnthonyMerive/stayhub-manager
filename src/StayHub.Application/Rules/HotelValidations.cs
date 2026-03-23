using StayHub.Domain.Entities;
using StayHub.Domain.Exceptions;

namespace StayHub.Application.Rules
{
    public static class HotelValidations
    {
        /// <summary>
        /// Sanitiza y valida los datos de entrada de un hotel
        /// </summary>
        public static void SanitizeHotelData(Hotel hotel, bool isUpdate = false)
        {
            // Validar que el objeto no sea null
            ArgumentNullException.ThrowIfNull(hotel);

            // Validar HotelId para actualizaciones
            if (isUpdate && hotel.HotelId <= 0)
            {
                throw new BusinessException("INVALID_HOTEL_ID",
                    "El ID del hotel debe ser un valor positivo.");
            }

            // Sanitizar y validar Nombre
            hotel.Nombre = SanitizeString(hotel.Nombre);
            if (string.IsNullOrWhiteSpace(hotel.Nombre))
            {
                throw new BusinessException("NOMBRE_REQUIRED",
                    "El nombre del hotel es obligatorio.");
            }

            // Remover espacios extra del nombre
            hotel.Nombre = hotel.Nombre.Trim();

            // Validar longitud del nombre
            if (hotel.Nombre.Length > 100)
            {
                throw new BusinessException("NOMBRE_TOO_LONG",
                    "El nombre del hotel no puede exceder los 100 caracteres.");
            }

            // Sanitizar y validar Direccion
            hotel.Direccion = SanitizeString(hotel.Direccion);
            if (string.IsNullOrWhiteSpace(hotel.Direccion))
            {
                throw new BusinessException("DIRECCION_REQUIRED",
                    "La dirección del hotel es obligatoria.");
            }

            // Remover espacios extra de la dirección
            hotel.Direccion = hotel.Direccion.Trim();

            // Validar longitud de la dirección
            if (hotel.Direccion.Length > 200)
            {
                throw new BusinessException("DIRECCION_TOO_LONG",
                    "La dirección del hotel no puede exceder los 200 caracteres.");
            }

            // Sanitizar y validar Ciudad
            hotel.Ciudad = SanitizeString(hotel.Ciudad);
            if (string.IsNullOrWhiteSpace(hotel.Ciudad))
            {
                throw new BusinessException("CIUDAD_REQUIRED",
                    "La ciudad del hotel es obligatoria.");
            }

            // Remover espacios extra de la ciudad
            hotel.Ciudad = hotel.Ciudad.Trim();

            // Validar longitud de la ciudad
            if (hotel.Ciudad.Length > 50)
            {
                throw new BusinessException("CIUDAD_TOO_LONG",
                    "La ciudad del hotel no puede exceder los 50 caracteres.");
            }

            // Note: Hotel entity only has Nombre, Ciudad, Direccion, Estado, FechaCreacion
            // No additional properties need validation beyond the basic ones
        }

        /// <summary>
        /// Sanitiza cadenas de texto para prevenir valores null y normalizar espacios
        /// </summary>
        public static string SanitizeString(string? input)
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
    }
}
