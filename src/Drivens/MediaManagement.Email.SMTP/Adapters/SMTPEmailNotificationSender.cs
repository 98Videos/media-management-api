using MediaManagement.Email.SMTP.Options;
using MediaManagementApi.Domain.Ports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace MediaManagement.Email.SMTP.Adapters
{
    public class SMTPEmailNotificationSender : INotificationSender
    {
        private readonly SMTPSenderOptions smtpOptions;
        private readonly SmtpClient smtpClient;
        private readonly ILogger<SMTPEmailNotificationSender> logger;

        public SMTPEmailNotificationSender(IOptions<SMTPSenderOptions> options, ILogger<SMTPEmailNotificationSender> logger)
        {
            smtpOptions = options.Value;

            smtpClient = new SmtpClient(smtpOptions.Host, smtpOptions.Port)
            {
                Credentials = new NetworkCredential(smtpOptions.User, smtpOptions.Password),
                EnableSsl = true
            };

            this.logger = logger;
        }

        public async Task<bool> SendNotification(string recipient, string subject, string message)
        {
            try
            {
                logger.LogInformation("sending notification email to {recipient}", recipient);

                await smtpClient.SendMailAsync(smtpOptions.FromEmail, recipient, subject, message);

                logger.LogInformation("sent successfuly");

                return true;
            }
            catch (Exception e)
            {
                const string errorMessage = "error sending email";

                logger.LogError(e, errorMessage);
                return false;
            }
        }
    }
}