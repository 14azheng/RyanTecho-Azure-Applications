using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using RyanTechno.AzureApps.Common.Interfaces.Network;
using RyanTechno.AzureApps.Common.Models;
using System.Diagnostics;
using System.Net.Http.Json;

namespace RyanTechno.AzureApps.Services.Network
{
    public class HttpRestService : IHttpRestService
    {
        private readonly ILogger<HttpRestService> _logger;

        public HttpRestService(ILogger<HttpRestService> logger)
        {
            _logger = logger;
        }

        public async Task<ServiceResult<string>> GetAccessTokenAsync(HttpClient httpClient, AuthenticationInfo info)
        {
            _logger.LogInformation($"Start getting access token...Endpoint: {info.AcquireAccessTokenEndpoint} | Client Id: {info.ClientId} | Client Secret: {info.ClientSecret} | Scope: {info.Scope}");

            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
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

        public async Task<ServiceResult<TResource>> GetResourcesAsync<TResource>(HttpClient httpClient, RestRequestInfo info)
        {
            _logger.LogInformation($"Start getting resources...Endpoint: {info.RequestEndpoint}");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, info.RequestEndpoint);
            // Construst request headers.
            foreach (var header in info.RequestHeaders)
            {
                httpRequestMessage.Headers.Add(header.Key, header.Value);
            }

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                TResource content = await httpResponseMessage.Content?.ReadFromJsonAsync<TResource>() ?? default(TResource);

                _logger.LogInformation($"End getting resources...");

                return ServiceResult<TResource>.FromSuccess(content);
            }
            else
            {
                _logger.LogError($"Get resources failed. Response code: {httpResponseMessage.StatusCode} | Error message: {httpResponseMessage.Content}");

                return ServiceResult<TResource>.FromFailure(default(TResource), httpResponseMessage.Content.ToString());
            }
        }

        public async Task<ServiceResult<byte[]>> GetStreamAsync(HttpClient httpClient, RestRequestInfo info)
        {
            _logger.LogInformation($"Start getting stream...Endpoint: {info.RequestEndpoint}");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, info.RequestEndpoint);
            // Construst request headers.
            foreach (var header in info.RequestHeaders)
            {
                httpRequestMessage.Headers.Add(header.Key, header.Value);
            }

            byte[] contents = Array.Empty<byte>();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string contentStr = await httpResponseMessage.Content.ReadAsStringAsync();
                if (contentStr.Contains("error", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogError($"Get stream failed. Response code: {httpResponseMessage.StatusCode} | Error message: {contentStr}");

                    return ServiceResult<byte[]>.FromFailure(contents, contentStr);
                }

                using Stream? stream = await httpResponseMessage.Content.ReadAsStreamAsync() ?? null;

                if (stream != null)
                {
                    using MemoryStream ms = new();
                    stream.CopyTo(ms);
                    contents = ms.ToArray();
                }
                else
                {
                    _logger.LogInformation("No content returns from endpoint.");
                }
                _logger.LogInformation($"End getting stream...");

                return ServiceResult<byte[]>.FromSuccess(contents);
            }
            else
            {
                _logger.LogError($"Get stream failed. Response code: {httpResponseMessage.StatusCode} | Error message: {httpResponseMessage.Content}");

                return ServiceResult<byte[]>.FromFailure(contents, httpResponseMessage.Content.ToString());
            }
        }
    }
}
