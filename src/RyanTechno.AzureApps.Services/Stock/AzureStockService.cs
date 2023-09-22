using Microsoft.Extensions.Logging;
using RyanTechno.AzureApps.Common.Interfaces.Network;
using RyanTechno.AzureApps.Common.Interfaces.Stock;
using RyanTechno.AzureApps.Common.Models;
using RyanTechno.AzureApps.Common.Models.Stock;
using RyanTechno.AzureApps.Domain.Stock;
using RyanTechno.AzureApps.Services.Extensions;

namespace RyanTechno.AzureApps.Services.Stock
{
    public class AzureStockService : IStockService
    {
        private readonly ILogger<AzureStockService> _logger;
        private readonly IHttpRestService _httpRestService;

        public AzureStockService(ILogger<AzureStockService> logger, IHttpRestService httpRestService) 
        {
            _logger = logger;
            _httpRestService = httpRestService;
        }

        public async Task<List<StockDaily>?> GetDailyStockInfoAsync(HttpClient httpClient, string requestUrl)
        {
            ServiceResult<DailyStockApiStructure> serviceResult = await _httpRestService.GetResourcesAsync<DailyStockApiStructure>(httpClient, new RestRequestInfo()
            {
                //RequestEndpoint = "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=603189.SHH&outputsize=full&apikey=NXMKD2BQGY8WWV58"
                RequestEndpoint = requestUrl,
            });

            if (serviceResult.IsCompleted)
            {
                return serviceResult.Result.ToModels();
            }
            else
            {
                _logger.LogError($"Failed in getting resource, error: {serviceResult.Error}");
                return null;
            }
        }
    }
}
