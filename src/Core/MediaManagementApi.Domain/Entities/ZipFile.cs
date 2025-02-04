namespace MediaManagementApi.Domain.Entities;

public record ZipFile
{
    public string Identifier { get; private set; }
    public Stream FileStreamReference { get; private set; }
    public ZipFile(string identifier, Stream fileStreamReference)
    {
        Identifier = identifier;
        FileStreamReference = fileStreamReference;
    }

}