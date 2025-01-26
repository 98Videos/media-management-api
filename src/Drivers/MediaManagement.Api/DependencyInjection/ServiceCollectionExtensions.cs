using MediaManagement.Api.Options;
using MediaManagement.Api.Services;
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

            var cognitoConfig = services.BuildServiceProvider().GetRequiredService<IOptions<CognitoAuthenticationOptions>>().Value;
            var userPoolId = cognitoConfig.UserPoolId;

            var cognitoUrlUserPoolId = userPoolId.Replace("_", "").ToLower();

            services.AddHttpClient<ICognitoUserInfoService, CognitoUserIdentityService>(client =>
            {
                client.BaseAddress = new Uri($"https://{cognitoUrlUserPoolId}.auth.us-east-1.amazoncognito.com/oauth2/userinfo");
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
                    options.Authority = $"https://cognito-idp.us-east-1.amazonaws.com/{userPoolId}";
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = $"https://cognito-idp.us-east-1.amazonaws.com/{userPoolId}",
                        ValidateLifetime = true,
                        ValidateAudience = false,
                    };
                });

            return services;
        }
    }
}