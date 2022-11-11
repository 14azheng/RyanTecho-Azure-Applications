using Microsoft.AspNetCore.Mvc;

namespace RyanTechno.AzureApps.Server.ExchangeApi.Helpers
{
    public class HttpHelper
    {
        public static async Task<JsonResult> RequestResources<T>(HttpClient httpClient, string url, Dictionary<string, string> headers)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            // Construst request headers.
            if (headers is not null)
            {
                foreach(var header in headers)
                {
                    httpRequestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                T? content = await httpResponseMessage.Content.ReadFromJsonAsync<T>();

                return new JsonResult(content);
            }
            else
            {
                return new JsonResult(httpRequestMessage.ToString());
            }
        }
    }
}
