namespace MediaManagement.S3.Exceptions;

public class FileRetrievalException: Exception
{
    public FileRetrievalException(string message, Exception innerException) 
    : base(message, innerException)
    {
    }
}