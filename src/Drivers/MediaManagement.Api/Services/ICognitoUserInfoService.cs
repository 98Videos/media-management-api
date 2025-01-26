using MediaManagement.Api.Models;
using System.Net.Http.Headers;

namespace MediaManagement.Api.Services
{
    public interface ICognitoUserInfoService
    {
        Task<UserInformation> GetUserInformationAsync(string userToken);
    }
}