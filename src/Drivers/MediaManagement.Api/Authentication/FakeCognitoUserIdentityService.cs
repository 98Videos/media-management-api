using MediaManagement.Api.Models;

namespace MediaManagement.Api.Authentication
{
    public class FakeCognitoUserIdentityService : ICognitoUserInfoService
    {
        public Task<UserInformation> GetUserInformationAsync(string userToken, CancellationToken cancellationToken = default) =>
            Task.FromResult(new UserInformation("test", "email@email.com"));
    }
}