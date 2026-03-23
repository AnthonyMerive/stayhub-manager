namespace StayHub.Domain.Exceptions;

/// <summary>
/// Excepción cuando no se encuentra una entidad
/// </summary>
public class NotFoundException : Exception
{
    public string EntityName { get; }
    public object? EntityId { get; }
    public static int HttpStatusCode => 404;

    public NotFoundException(string entityName, object? entityId) 
        : base($"La entidad '{entityName}' con identificador '{entityId}' no fue encontrada.")
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    public NotFoundException(string entityName, object? entityId, Exception innerException) 
        : base($"La entidad '{entityName}' con identificador '{entityId}' no fue encontrada.", innerException)
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}
