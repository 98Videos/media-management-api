using MediaManagement.Api.Authentication.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace MediaManagement.Api.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string HeaderName = "x-api-key";

        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options,
                                           ILoggerFactory logger,
                                           UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(HeaderName, out var apiKeyValues))
            {
                return AuthenticateResult.Fail("Missing API Key");
            }

            var acceptedKeys = base.OptionsMonitor.CurrentValue
            .Keys.Split(";")
            .ToDictionary(pair => pair.Split(":")[1], pair => pair.Split(":")[0]);

            var providedApiKey = apiKeyValues.FirstOrDefault();

            if (string.IsNullOrEmpty(providedApiKey) || !acceptedKeys.ContainsKey(providedApiKey))
            {
                return AuthenticateResult.Fail("Invalid API Key");
            }

            var claims = new[] { new Claim(ClaimTypes.Name, acceptedKeys[providedApiKey]) };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}