namespace MediaManagement.S3.Exceptions;

public class FileUploadException : Exception
{
    public FileUploadException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
