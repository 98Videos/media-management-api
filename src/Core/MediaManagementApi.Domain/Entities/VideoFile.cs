namespace MediaManagementApi.Domain.Entities;

public class VideoFile : ProcessFile
{
    public VideoFile(string identifier, Stream fileStreamReference) : base(identifier, fileStreamReference)
    {
    }
}