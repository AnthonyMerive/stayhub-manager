namespace StayHub.Domain.Exceptions;

/// <summary>
/// Excepción para violaciones de reglas de negocio
/// </summary>
public class BusinessException : Exception
{
    public string RuleCode { get; }
    public int HttpStatusCode { get; }

    public BusinessException(string ruleCode, string message, int httpStatusCode = 400) : base(message)
    {
        RuleCode = ruleCode;
        HttpStatusCode = httpStatusCode;
    }

    public BusinessException(string ruleCode, string message, Exception innerException, int httpStatusCode = 400) 
        : base(message, innerException)
    {
        RuleCode = ruleCode;
        HttpStatusCode = httpStatusCode;
    }
}
