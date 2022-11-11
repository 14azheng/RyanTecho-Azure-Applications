using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using RyanTechno.AzureApps.Server.ExchangeApi.Helpers;
using RyanTechno.AzureApps.Server.ExchangeApi.Models;

namespace RyanTechno.AzureApps.Server.ExchangeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CurrencyController : ControllerBase
    {
        private readonly ILogger<CurrencyController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CurrencyController(ILogger<CurrencyController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves live exchange rate for specified currencies.
        /// </summary>
        /// <param name="source">Source currency</param>
        /// <param name="targets">Target currencies</param>
        /// <returns>Exchange rates</returns>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     Get /GetLiveExchangeRate
        ///     {
        ///        "source": "CNY",
        ///        "targets": "USD,HKD,JPY,EUR,MOP,GBP,AUD"
        ///     }
        ///     
        /// </remarks>
        [HttpGet(Name = "GetLiveExchangeRate")]
        public async Task<JsonResult> Get(string source, string targets)
        {
            _logger.LogInformation($"New request (GetLiveExchangeRate) is coming...Source: {source} | Target: {targets}");

            string liveUrl = _configuration["APILayer:URL:Live"];
            liveUrl = $"{liveUrl}?source={source}";
            if (!string.IsNullOrEmpty(targets))
                liveUrl = $"{liveUrl}&currencies={targets}";

            var headers = new Dictionary<string, string>()
            {
                { HeaderNames.Accept, "*/*" },
                { "apikey", "WMzn6UGAogv4bk1Ky2e55N5uIZb2XjEF" }
            };

            var httpClient = _httpClientFactory.CreateClient();

            return await HttpHelper.RequestResources<CurrencyData>(httpClient, liveUrl, headers);
        }
    }
}
