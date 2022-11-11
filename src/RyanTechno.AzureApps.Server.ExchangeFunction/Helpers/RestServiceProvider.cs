using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace RyanTechno.AzureApps.Server.ExchangeFunction.Helpers
{
    internal class RestServiceProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public RestServiceProvider(ILogger logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public async Task<(bool Success, string AccessToken)> GetAccessTokenAsync(AuthenticationInfo info)
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
                return (false, string.Empty);
            }

            _logger.LogInformation($"End getting access token...Access token: {tokenResponse.AccessToken}");

            return (true, tokenResponse.AccessToken);
        }

        public async Task<(bool Success, T Resource)> GetResourcesAsync<T>(RestRequestInfo info)
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
                T content = await httpResponseMessage.Content.ReadFromJsonAsync<T>();

                _logger.LogInformation($"End getting resources...Response Json:");
                _logger.LogInformation(await httpResponseMessage.Content.ReadAsStringAsync());

                return (true, content);
            }
            else
            {
                _logger.LogError($"Get resources failed. Response code: {httpResponseMessage.StatusCode} | Error message: {httpResponseMessage.Content}");

                return (false, default(T));
            }
        }
    }

    public class AuthenticationInfo
    {
        public string AcquireAccessTokenEndpoint { get; init; }

        public string ClientId { get; init; }

        public string ClientSecret { get; init; }

        public string Scope { get; init; }
    }

    public class RestRequestInfo
    {
        public string RequestEndpoint { get; init; }

        public Dictionary<string, string> RequestHeaders { get; init; } = new();
    }
}
