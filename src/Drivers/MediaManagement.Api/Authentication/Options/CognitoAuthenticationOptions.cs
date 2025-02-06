namespace MediaManagement.Api.Authentication.Options
{
    public class CognitoAuthenticationOptions
    {
        public string UserPoolId { get; set; } = null!;
        public string CognitoDomain { get; set; } = null!;
    }
}
