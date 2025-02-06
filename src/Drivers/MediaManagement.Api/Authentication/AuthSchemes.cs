using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MediaManagement.Api.Authentication
{
    public class AuthSchemes
    {

        public const string BearerTokenAndApiKey = JwtBearerDefaults.AuthenticationScheme + "," + AuthSchemes.ApiKeyOnly;
        public const string BearerToken = JwtBearerDefaults.AuthenticationScheme;
        public const string ApiKeyOnly = "ApiKey";
    }
}
