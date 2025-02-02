namespace MediaManagementApi.Domain.Entities;

public record ZipFile
{
    private string Identifier { get; set; }
    private ZipFile() { }
    public ZipFile(string identifier)
    {
        Identifier = identifier;
    }
}