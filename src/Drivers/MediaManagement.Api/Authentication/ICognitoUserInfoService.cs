using MediaManagement.Api.Models;

namespace MediaManagement.Api.Authentication
{
    public interface ICognitoUserInfoService
    {
        Task<UserInformation> GetUserInformationAsync(string userToken, CancellationToken cancellationToken = default);
    }
}