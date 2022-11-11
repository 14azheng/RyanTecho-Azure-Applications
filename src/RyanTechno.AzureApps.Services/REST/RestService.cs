using IdentityModel.Client;
using RyanTechno.AzureApps.Domain.Rest;
using System.Net.Http.Json;

namespace RyanTechno.AzureApps.Services.REST
{
    public class RestService : IRestService
    {
        private readonly HttpClient _httpClient;

        public RestService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<(bool Success, string AccessToken)> GetAccessTokenAsync(AuthenticationInfo info)
        {
            var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = info.AcquireAccessTokenEndpoint,
                ClientId = info.ClientId,
                ClientSecret = info.ClientSecret,
                Scope = info.Scope,
            });

            if (tokenResponse.IsError)
            {
                return (false, string.Empty);
            }

            return (true, tokenResponse.AccessToken);
        }

        public async Task<(bool Success, T Resource)> GetResourcesAsync<T>(RestRequestInfo info)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, info.RequestEndpoint);
            // Construst request headers.
            foreach (var header in info.RequestHeaders)
            {
                httpRequestMessage.Headers.Add(header.Key, header.Value);
            }

            var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                T content = await httpResponseMessage.Content.ReadFromJsonAsync<T>();

                return (true, content);
            }
            else
            {
                return (false, default(T));
            }
        }
    }
}
