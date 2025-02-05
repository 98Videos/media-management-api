using MediaManagement.Api.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MediaManagement.Api.Services
{
    public class CognitoUserIdentityService : ICognitoUserInfoService
    {
        private readonly HttpClient _httpClient;

        private readonly JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

        public CognitoUserIdentityService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserInformation> GetUserInformationAsync(string userToken, CancellationToken cancellationToken = default)
        {
            var requestMessage = new HttpRequestMessage() 
            { 
                RequestUri = new Uri($"{_httpClient.BaseAddress}oauth2/userinfo"),
                Method = HttpMethod.Get 
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: userToken);

            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            var userInfo = await JsonSerializer.DeserializeAsync<UserInfoResponse>(response.Content.ReadAsStream(), jsonSerializerOptions, cancellationToken)
                ?? throw new Exception("Error getting user info!");

            return new UserInformation(userInfo.Username, userInfo.Email);
        }

        private class UserInfoResponse
        {
            public required string Sub { get; set; }
            public required string EmailVerified { get; set; }
            public required string Email { get; set; }
            public required string Username { get; set; }
        }
    }
}