using MediaManagement.Api.Options;
using MediaManagement.Api.Services;
using MediaManagement.Application.UseCases;
using MediaManagement.Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MediaManagement.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCognitoAuthentication(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                services.AddScoped<ICognitoUserInfoService, FakeCognitoUserIdentityService>();
                return services;
            }

            services.Configure<CognitoAuthenticationOptions>(configuration.GetSection(nameof(CognitoAuthenticationOptions)));
            services.AddScoped<IImageUseCase, ImageUseCase>();

            var cognitoConfig = services.BuildServiceProvider().GetRequiredService<IOptions<CognitoAuthenticationOptions>>().Value;

            services.AddHttpClient<ICognitoUserInfoService, CognitoUserIdentityService>(client =>
            {
                client.BaseAddress = new Uri(cognitoConfig.CognitoDomain[^1] == '/' ? cognitoConfig.CognitoDomain : cognitoConfig.CognitoDomain + '/');
            });

            services
                .AddAuthorization()
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = $"https://cognito-idp.us-east-1.amazonaws.com/{cognitoConfig.UserPoolId}";
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = $"https://cognito-idp.us-east-1.amazonaws.com/{cognitoConfig.UserPoolId}",
                        ValidateLifetime = true,
                        ValidateAudience = false,
                    };
                });

            return services;
        }
    }
}