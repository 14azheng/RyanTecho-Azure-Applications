using RyanTechno.AzureApps.Common.Interfaces.Network;
using RyanTechno.AzureApps.Common.Models;
using RyanTechno.AzureApps.Domain.Stock;
using RyanTechno.AzureApps.Infrastructure.Logging;
using RyanTechno.AzureApps.Services.Network;
using static System.Net.WebRequestMethods;

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

        public static async Task RequestDailyCsv(string stockMarket, string stockCode, string filePath, string? accessToken = null)
        {
            if (accessToken is null)
            {
                accessToken = await RetrieveAccessToken();
            }

            if (accessToken is not null)
            {
                HttpClient httpClient = new();
                IHttpRestService httpRestService = new HttpRestService(new ConsoleLogger<HttpRestService>());
                // Get daily stock information from Web API.
                var stockTask = await httpRestService.GetStreamAsync(httpClient, new RestRequestInfo
                {
                    RequestEndpoint = $"https://stock.ryantechno.com/api/stock/GetDailyStockCsv?stockMarket={stockMarket}&stockCode={stockCode}&outputSize=full",
                    //RequestEndpoint = "https://localhost:7201/api/stock/GetDailyStockCsv?stockMarket=SHH&stockCode=603189&outputSize=full",
                    RequestHeaders = new Dictionary<string, string>
                    {
                        { "Authorization", "Bearer " + accessToken }
                    }
                });

                if (stockTask.IsCompleted)
                {
                    // Save csv file to local disk.
                    //string filePath = Path.Combine(Environment.CurrentDirectory, $"{Guid.NewGuid().ToString()}.csv");
                    using FileStream fs = new(filePath, FileMode.Create, FileAccess.Write);
                    fs.Write(stockTask.Result, 0, stockTask.Result.Length);
                    fs.Flush();
                    System.Console.WriteLine($"File saved to: {filePath}");
                }
                else
                {
                    System.Console.WriteLine($"Error: {stockTask.Error}");
                }
            }
        }

        public static async Task<string?> RetrieveAccessToken()
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
                return tokenTask.Result;
            }
            else
            {
                System.Console.WriteLine($"Error: {tokenTask.Error}");
                return null;
            }
        }
    }
}
