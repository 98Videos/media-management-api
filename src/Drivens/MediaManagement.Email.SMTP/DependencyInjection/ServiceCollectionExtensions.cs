using MediaManagement.Email.SMTP.Adapters;
using MediaManagement.Email.SMTP.Options;
using MediaManagementApi.Domain.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaManagement.Email.SMTP.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSMTPEmailSender(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SMTPSenderOptions>(configuration.GetSection(nameof(SMTPSenderOptions)));
            services.AddScoped<INotificationSender, SMTPEmailNotificationSender>();

            return services;
        }
    }
}