namespace MediaManagement.S3.Exceptions;

public class FileDownloadException : Exception
{
    public FileDownloadException(string message, Exception innerException) : base(message, innerException) { }

    public FileDownloadException(string message) : base(message) { }
}
