using MediaManagementApi.Domain.Enums;
using MediaManagementApi.Domain.Exceptions;

namespace MediaManagementApi.Domain.Entities;

public class Video
{
    public Guid Id { get; private set; }
    public string EmailUser { get; set; }
    public string Filename { get; set; }
    public VideoStatus Status { get; set; }

    public Video(string emailUser, string filename)
    {
        Id = Guid.NewGuid();
        EmailUser = emailUser;
        Filename = filename;
        Status = VideoStatus.EmProcessamento;
    }

    private void ValidateEntity()
    {
        if (string.IsNullOrEmpty(EmailUser))
            throw new DomainValidationException(nameof(EmailUser));
        if (string.IsNullOrEmpty(Filename))
            throw new DomainValidationException(nameof(Filename));
    }
    
    public void UpdateStatus(VideoStatus status) => Status = status;
}