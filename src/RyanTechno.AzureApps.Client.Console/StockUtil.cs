using RyanTechno.AzureApps.Common.Interfaces.Network;
using RyanTechno.AzureApps.Common.Models;
using RyanTechno.AzureApps.Domain.Stock;
using RyanTechno.AzureApps.Infrastructure.Logging;
using RyanTechno.AzureApps.Services.Network;

namespace RyanTechno.AzureApps.Client.Console
{
    internal class StockUtil
    {
        public static async Task RequestDailyInfo()
        {
            HttpClient httpClient = new();
            IHttpRestService httpRestService = new HttpRestService(new ConsoleLogger<HttpRestService>());
            var tokenTask = await httpRestService.GetAccessTokenAsync(httpClient, new AuthenticationInfo
            {
                AcquireAccessTokenEndpoint = "https://auth.ryantechno.com/connect/token",
                ClientId = "stock_api",
                ClientSecret = "94TQ7n7f8E",
                Scope = "stock",
            });

            if (tokenTask.IsCompleted)
            {
                string accessToken = tokenTask.Result;
                // Get daily stock information from Web API.
                var stockTask = await httpRestService.GetResourcesAsync<List<StockDaily>?>(httpClient, new RestRequestInfo
                {
                    RequestEndpoint = "https://stock.ryantechno.com/api/stock/GetDailyStock?stockMarket=SHH&stockCode=603189",
                    RequestHeaders = new Dictionary<string, string>
                    {
                        { "Authorization", "Bearer " + accessToken }
                    }
                });

                if (stockTask.IsCompleted)
                {
                    System.Console.WriteLine(string.Join('|', stockTask.Result));
                }
                else
                {
                    System.Console.WriteLine($"Error: {stockTask.Error}");
                }
            }
            else
            {
                System.Console.WriteLine($"Error: {tokenTask.Error}");
            }
        }
    }
}
