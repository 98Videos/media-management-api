namespace MediaManagement.Email.SMTP.Options
{
    public class SMTPSenderOptions
    {
        public required string Host { get; set; }
        public required int Port { get; set; }
        public required string User { get; set; }
        public required string Password { get; set; }
        public required string FromEmail { get; set; }
    }
}