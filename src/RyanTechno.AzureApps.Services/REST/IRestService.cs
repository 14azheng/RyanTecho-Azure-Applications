using RyanTechno.AzureApps.Domain.Rest;

namespace RyanTechno.AzureApps.Services.REST
{
    public interface IRestService
    {
        Task<(bool Success, string AccessToken)> GetAccessTokenAsync(AuthenticationInfo info);

        Task<(bool Success, T Resource)> GetResourcesAsync<T>(RestRequestInfo info);
    }
}
