using MediaManagement.Api.Models;

namespace MediaManagement.Api.Services
{
    public class FakeCognitoUserIdentityService : ICognitoUserInfoService
    {
        public Task<UserInformation> GetUserInformationAsync(string userToken, CancellationToken cancellationToken = default) =>
            Task.FromResult(new UserInformation("test", "email@email.com"));
    }
}