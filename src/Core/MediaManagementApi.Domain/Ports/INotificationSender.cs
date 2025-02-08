using MediaManagementApi.Domain.Enums;

namespace MediaManagementApi.Domain.Ports
{
    public interface INotificationSender
    {
        public Task<bool> SendNotification(string subject, string recipient, string message);
    }
}