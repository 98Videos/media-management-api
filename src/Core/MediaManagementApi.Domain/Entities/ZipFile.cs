namespace MediaManagementApi.Domain.Entities;

public record ZipFile
{
    public string Identifier { get; private set; }
    private ZipFile() { }
    public ZipFile(string identifier)
    {
        Identifier = identifier;
    }
}