namespace MediaManagementApi.Domain.Entities;

public class ProcessFile
{
    public string Identifier { get; private set; }
    public Stream FileStreamReference { get; private set; }

    public ProcessFile(string identifier, Stream fileStreamReference)
    {
        Identifier = identifier;
        FileStreamReference = fileStreamReference;
    }
}