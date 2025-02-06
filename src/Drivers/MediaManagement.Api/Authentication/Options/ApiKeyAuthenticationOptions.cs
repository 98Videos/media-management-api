using Microsoft.AspNetCore.Authentication;

namespace MediaManagement.Api.Authentication.Options
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Keys { get; set; }
    }
}