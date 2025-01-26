using MediaManagement.Api.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MediaManagement.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCognitoAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var cognitoConfig = new CognitoAuthenticationOptions();
            configuration
                .GetRequiredSection(nameof(CognitoAuthenticationOptions))
                .Bind(cognitoConfig);

            var userPoolId = cognitoConfig.UserPoolId;

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