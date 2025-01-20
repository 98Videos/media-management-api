using MediaManagementApi.Domain.Enums;

namespace MediaManagementApi.Domain.Entities;

public class Video
{
    public Guid Id { get; private set; }
    public string EmailUser { get; set; }
    public string Filename { get; set; }
    public VideoStatus Status { get; set; }

    public Video(Guid id, string emailUser, string filename, VideoStatus status)
    {
        Id = id;
        EmailUser = emailUser;
        Filename = filename;
        Status = status;
    }
    
    public void UpdateStatus(VideoStatus status) => Status = status;
}