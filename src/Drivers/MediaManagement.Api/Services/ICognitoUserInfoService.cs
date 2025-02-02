using MediaManagement.Api.Models;

namespace MediaManagement.Api.Services
{
    public interface ICognitoUserInfoService
    {
        Task<UserInformation> GetUserInformationAsync(string userToken, CancellationToken cancellationToken = default);
    }
}