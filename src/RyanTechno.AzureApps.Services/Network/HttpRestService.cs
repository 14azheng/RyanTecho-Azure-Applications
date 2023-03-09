using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using RyanTechno.AzureApps.Common.Interfaces.Network;
using RyanTechno.AzureApps.Common.Models;
using System.Net.Http.Json;

namespace RyanTechno.AzureApps.Services.Network
{
    public class HttpRestService : IHttpRestService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpRestService> _logger;

        public HttpRestService(HttpClient httpClient, ILogger<HttpRestService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> GetAccessTokenAsync(AuthenticationInfo info)
        {
            _logger.LogInformation($"Start getting access token...Endpoint: {info.AcquireAccessTokenEndpoint} | Client Id: {info.ClientId} | Client Secret: {info.ClientSecret} | Scope: {info.Scope}");

            var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = info.AcquireAccessTokenEndpoint,
                ClientId = info.ClientId,
                ClientSecret = info.ClientSecret,
                Scope = info.Scope,
            });

            if (tokenResponse.IsError)
            {
                _logger.LogError($"Get access token failed. Error message: {tokenResponse.Error}");

                return ServiceResult<string>.FromFailure(string.Empty, tokenResponse.Error);
            }

            _logger.LogInformation($"End getting access token...Access token: {tokenResponse.AccessToken}");

            return ServiceResult<string>.FromSuccess(tokenResponse.AccessToken);
        }

        public async Task<ServiceResult<TResource>> GetResourcesAsync<TResource>(RestRequestInfo info)
        {
            _logger.LogInformation($"Start getting resources...Endpoint: {info.RequestEndpoint}");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, info.RequestEndpoint);
            // Construst request headers.
            foreach (var header in info.RequestHeaders)
            {
                httpRequestMessage.Headers.Add(header.Key, header.Value);
            }

            var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                TResource content = await httpResponseMessage.Content?.ReadFromJsonAsync<TResource>() ?? default(TResource);

                _logger.LogInformation($"Start getting resources...");

                return ServiceResult<TResource>.FromSuccess(content);
            }
            else
            {
                _logger.LogError($"Get resources failed. Response code: {httpResponseMessage.StatusCode} | Error message: {httpResponseMessage.Content}");

                return ServiceResult<TResource>.FromFailure(default(TResource), httpResponseMessage.Content.ToString());
            }
        }
    }
}
