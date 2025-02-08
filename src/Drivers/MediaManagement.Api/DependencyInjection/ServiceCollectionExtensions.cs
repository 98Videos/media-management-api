using MediaManagement.Api.Authentication;
using MediaManagement.Api.Authentication.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Threading.RateLimiting;

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

        public static IServiceCollection ConfigureRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(_ => _
                .AddFixedWindowLimiter(policyName: "fixed", options =>
                {
                    options.PermitLimit = 30;
                    options.Window = TimeSpan.FromSeconds(20);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 10;
                }));

            return services;
        }

        public static IServiceCollection ConfigureLogging(this IServiceCollection services)
        {
            services.AddSerilog(cfg =>
            {
                cfg
                    .WriteTo.Console()
                    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning);
            });

            return services;
        }
    }
}