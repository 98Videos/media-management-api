namespace MediaManagement.Api.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetJwtBearerToken(this HttpRequest request) =>
            request.Headers.Authorization.ToString().Replace("Bearer ", "");
    }
}
