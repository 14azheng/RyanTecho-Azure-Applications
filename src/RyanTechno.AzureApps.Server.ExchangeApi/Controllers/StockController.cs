using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RyanTechno.AzureApps.Common.Interfaces.Network;
using RyanTechno.AzureApps.Common.Interfaces.Stock;

namespace RyanTechno.AzureApps.Server.ExchangeApi.Controllers
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

        [HttpGet(Name = "GetDailyStockInformation")]
        [Route("[action]")]
        public async Task<JsonResult> GetDailyStock(string stockMarket, string stockCode)
        {
            //string url = "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=603189.SHH&outputsize=full&apikey=NXMKD2BQGY8WWV58";
            string url = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={stockCode}.{stockMarket}&outputsize=compact&apikey=NXMKD2BQGY8WWV58";
            var dailyInfo = await _stockService.GetDailyStockInfoAsync(_httpClientFactory.CreateClient(), url);

            return new JsonResult(dailyInfo);
        }
    }
}
