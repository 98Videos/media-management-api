using MediaManagement.Api.Authentication;
using MediaManagement.Api.Authentication.Options;
using MediaManagement.Application.UseCases;
using MediaManagement.Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MediaManagement.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCognitoAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CognitoAuthenticationOptions>(configuration.GetSection(nameof(CognitoAuthenticationOptions)));
            services.Configure<ApiKeyAuthenticationOptions>(configuration.GetSection(nameof(ApiKeyAuthenticationOptions)));

            var cognitoConfig = services.BuildServiceProvider().GetRequiredService<IOptions<CognitoAuthenticationOptions>>().Value;

            services.AddHttpClient<ICognitoUserInfoService, CognitoUserIdentityService>(client =>
            {
                client.BaseAddress = new Uri(cognitoConfig.CognitoDomain[^1] == '/' ? cognitoConfig.CognitoDomain : cognitoConfig.CognitoDomain + '/');
            });

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(AuthSchemes.ApiKeyOnly, null)
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

            services
                .AddAuthorizationBuilder()
                .AddPolicy("ApiKey", policy =>
                {
                    policy.AuthenticationSchemes.Add(AuthSchemes.ApiKeyOnly);
                    policy.RequireAuthenticatedUser();
                })
                .AddPolicy("Bearer", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });

            return services;
        }
    }
}