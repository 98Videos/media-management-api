namespace MediaManagementApi.Domain.Entities;

public record ZipFile
{
    public string FileName { get; private set; }
    public Stream FileStreamReference { get; private set; }
    public ZipFile(string fileName, Stream fileStreamReference)
    {
        FileName = fileName;
        FileStreamReference = fileStreamReference;
    }

}