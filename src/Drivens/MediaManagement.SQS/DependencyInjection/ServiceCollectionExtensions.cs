﻿using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using MassTransit;
using MediaManagement.SQS.Adapters;
using MediaManagement.SQS.Contracts;
using MediaManagementApi.Domain.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace MediaManagement.SQS.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqsMessagePublisher(this IServiceCollection services)
        {
            services.AddMassTransit(massTransitCfg =>
            {

                massTransitCfg.UsingAmazonSqs((context, sqsCfg) =>
                {
                    var credentialChain = new CredentialProfileStoreChain();
                    if (!credentialChain.TryGetAWSCredentials("default", out AWSCredentials awsCredentials))
                    {
                        awsCredentials = new EnvironmentVariablesAWSCredentials();
                    }

                    sqsCfg.Host("us-east-1", hostCfg =>
                    {
                        hostCfg.Credentials(awsCredentials);
                    });

                    EndpointConvention.Map<VideoToProcessMessage>(new Uri("queue:videos-to-process"));
                });
            });

            services.AddScoped<IMessagePublisher, SQSMessagePublisher>();

            return services;
        }
    }
}