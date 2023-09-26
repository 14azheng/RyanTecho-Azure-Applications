using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RyanTechno.AzureApps.Common.Interfaces.Network;
using RyanTechno.AzureApps.Common.Interfaces.Stock;
using RyanTechno.AzureApps.Infrastructure.Helpers;

namespace RyanTechno.AzureApps.Server.StockApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StockController : ControllerBase
    {
        private readonly ILogger<StockController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpRestService _httpRestService;
        private readonly IStockService _stockService;

        public StockController(ILogger<StockController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment,
            IHttpRestService httpRestService,
            IStockService stockService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpRestService = httpRestService;
            _stockService = stockService;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Get daily info of a specified stock.
        /// </summary>
        /// <param name="stockMarket">Market code. For China, it's SHH and SHZ</param>
        /// <param name="stockCode">Stock code.</param>
        /// <returns>A list of <see cref="List{StockDaily}"/> instances.</returns>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     Get /api/stock/getdailystock?stockmarket=SHH&stockcode=603189
        ///     
        /// </remarks>
        [HttpGet(Name = "GetDailyStockInformation")]
        [Route("[action]")]
        public async Task<JsonResult> GetDailyStock(string stockMarket, string stockCode)
        {
            _logger.LogInformation($"New request (GetDailyStock) is coming...Stock Market: {stockMarket} | Stock Code: {stockCode}");
            //string url = "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=603189.SHH&outputsize=full&apikey=NXMKD2BQGY8WWV58";

            _logger.LogInformation($"Starting to get external service API key...Key vault url: {_configuration["KeyVault:Url"]}");
            string apiKey = await KeyVaultHelper.GetSecretAsync(_configuration["KeyVault:Url"], "external-service-api-key");
            string url = $"{_configuration["StockData:ProviderUrl:TimeSeriesDaily"]}&symbol={stockCode}.{stockMarket}&outputsize=compact&apikey={apiKey}";
            _logger.LogInformation($"Starting to get stock data from external service...URL: {url}");
            var dailyInfo = await _stockService.GetDailyStockInfoAsync(_httpClientFactory.CreateClient(), url);

            return new JsonResult(dailyInfo);
        }
    }
}
