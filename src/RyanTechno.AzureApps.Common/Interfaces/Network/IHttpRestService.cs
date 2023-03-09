using RyanTechno.AzureApps.Common.Models;

namespace RyanTechno.AzureApps.Common.Interfaces.Network
{
    public interface IHttpRestService
    {
        Task<ServiceResult<string>> GetAccessTokenAsync(AuthenticationInfo info);

        Task<ServiceResult<TResource>> GetResourcesAsync<TResource>(RestRequestInfo info);
    }
}
