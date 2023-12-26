using RyanTechno.AzureApps.Common.Models;

namespace RyanTechno.AzureApps.Common.Interfaces.Network
{
    public interface IHttpRestService
    {
        Task<ServiceResult<string>> GetAccessTokenAsync(HttpClient httpClient, AuthenticationInfo info);

        Task<ServiceResult<TResource>> GetResourcesAsync<TResource>(HttpClient httpClient, RestRequestInfo info);

        Task<ServiceResult<byte[]>> GetStreamAsync(HttpClient httpClient, RestRequestInfo info);
    }
}
