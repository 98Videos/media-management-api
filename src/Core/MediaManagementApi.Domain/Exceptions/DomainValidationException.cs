namespace MediaManagementApi.Domain.Exceptions;

[Serializable]
public class DomainValidationException: Exception
{
    private const string MessageTemplate = "invalid value for {0}!";

    public DomainValidationException(string propertyName) : base(string.Format(MessageTemplate, propertyName))
    {
        
        
    }
}