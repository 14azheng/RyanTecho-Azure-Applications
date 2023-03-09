using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using RyanTechno.AzureApps.Common.Interfaces.Network;
using RyanTechno.AzureApps.Common.Models;
using RyanTechno.AzureApps.Infrastructure.Helpers;
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
        private readonly IHttpRestService _httpRestService;

        public CurrencyController(ILogger<CurrencyController> logger, 
            IHttpClientFactory httpClientFactory, 
            IConfiguration configuration,
            IHttpRestService httpRestService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpRestService = httpRestService;
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

            string liveUrl = _configuration["APILayer:Url:Live"];
            liveUrl = $"{liveUrl}?source={source}";
            if (!string.IsNullOrEmpty(targets))
                liveUrl = $"{liveUrl}&currencies={targets}";

            var headers = new Dictionary<string, string>()
            {
                { HeaderNames.Accept, "*/*" },
                { "apikey", await KeyVaultHelper.GetSecretAsync(_configuration["KeyVault:Url"], "external-service-api-key") }
            };

            var httpClient = _httpClientFactory.CreateClient();

            RestRequestInfo requestInfo = new()
            {
                RequestEndpoint = liveUrl,
                RequestHeaders = headers,
            };
            
            var result = await _httpRestService.GetResourcesAsync<ExternalCurrencyApiStructure>(requestInfo);

            if (result.IsCompleted)
            {
                return new JsonResult(result.Result);
            }
            else
            {
                _logger.LogError($"Failed in getting resource, error: {result.Error}");
                return new JsonResult(result.Error);
            }
        }
    }
}
