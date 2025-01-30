using Amazon.Runtime;
using MassTransit;
using MediaManagement.Application.MessageContracts;
using MediaManagement.SQS.Adapters;
using MediaManagementApi.Domain.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaManagement.SQS.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqsMessagePublisher(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.UsingAmazonSqs((context, cfg) =>
                {
                    cfg.Host("us-east-1", h =>
                    {
                        h.Credentials(new EnvironmentVariablesAWSCredentials());
                    });
                });
            });

            services.AddScoped<IMessagePublisher<VideoToProcessMessage>, SQSMessagePublisher<VideoToProcessMessage>>();

            return services;
        }
    }
}