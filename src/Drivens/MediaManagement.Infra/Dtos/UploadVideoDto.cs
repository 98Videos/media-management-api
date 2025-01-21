namespace MediaManagement.Database.Dtos;

public class UploadVideoDto
{
    public string UserEmail { get; set; }
    public Stream FileStream { get; set; }
}