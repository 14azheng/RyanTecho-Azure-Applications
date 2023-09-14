using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using RyanTechno.AzureApps.Common.Interfaces.Exchange;
using RyanTechno.AzureApps.Common.Interfaces.Network;
using RyanTechno.AzureApps.Common.Models;
using RyanTechno.AzureApps.Common.Models.Exchange;
using RyanTechno.AzureApps.Domain.Exchange;
using RyanTechno.AzureApps.Infrastructure.Helpers;
using RyanTechno.AzureApps.Services.Extensions;
using System.Collections.Immutable;

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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpRestService _httpRestService;
        private readonly IExchangeService _exchangeService;

        public CurrencyController(ILogger<CurrencyController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment,
            IHttpRestService httpRestService,
            IExchangeService exchangeService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpRestService = httpRestService;
            _exchangeService = exchangeService;
            _webHostEnvironment = webHostEnvironment;
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
        ///     Get /api/currency/getliverate?source=CNY&targets=HKD,USD,JPY
        ///     
        /// </remarks>
        [HttpGet(Name = "GetLiveExchangeRate")]
        [Route("[action]")]
        public async Task<JsonResult> GetLiveRate(string source, string? targets)
        {
            _logger.LogInformation($"New request (GetLiveExchangeRate) is coming...Source: {source} | Target: {targets}");

            ExternalLiveRateApiStructure liveRate = await RequestLiveExchangeRateAsync(source, targets);

            return new JsonResult(liveRate);
        }

        /// <summary>
        /// Retrieves daily exchange rate boardcast news.
        /// </summary>
        /// <param name="targets">Target currencies.</param>
        /// <param name="translate">Whether to translate into Chinese.</param>
        /// <returns>Exchange rates</returns>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     Get /api/currency/getdailyboardcast?targets=HKD,USD,JPY
        ///     
        /// </remarks>
        [HttpGet(Name = "GetDailyExchangeRateBoardcast")]
        [Route("[action]")]
        public async Task<JsonResult> GetDailyBoardcast(string? targets, bool? translate)
        {
            _logger.LogInformation($"New request (GetDailyExchangeRateBoardcast) is coming...Target: {targets ?? "All"}");

            try
            {
                // Initial historical exchange rate data.
                string folderPath = Path.Combine(_webHostEnvironment.ContentRootPath, _configuration["ExchangeRates:HistoricalData:OfflineFolder"]);
                var fileRawData = _exchangeService.GetHistoricalExchangeRatesFromFiles(folderPath);
                IImmutableDictionary<string, HistoricalExchangeRateSummary> historicalExchangeRates = _exchangeService.SummarizeHistoricalExchangeRates(fileRawData);

                ExternalLiveRateApiStructure liveRate = await RequestLiveExchangeRateAsync("CNY", targets);

                DailyExchangeRateBoardcastApiStructure boardcast = new DailyExchangeRateBoardcastApiStructure();
                decimal lowestRate90, lowestRate80, lowestRate70, highestRate90, highestRate80, highestRate70;
                // Compare live rate and historical rates.
                if (liveRate is not null && liveRate.Source == "CNY")
                {
                    foreach (var quote in liveRate.Quotes)
                    {
                        if (historicalExchangeRates.ContainsKey(quote.Key))
                        {
                            HistoricalExchangeRateSummary history = historicalExchangeRates[quote.Key];
                            lowestRate90 = history.LowestRate + (history.HighestRate - history.LowestRate) * 0.1M;
                            lowestRate80 = history.LowestRate + (history.HighestRate - history.LowestRate) * 0.2M;
                            lowestRate70 = history.LowestRate + (history.HighestRate - history.LowestRate) * 0.3M;
                            highestRate90 = history.HighestRate - (history.HighestRate - history.LowestRate) * 0.1M;
                            highestRate80 = history.HighestRate - (history.HighestRate - history.LowestRate) * 0.2M;
                            highestRate70 = history.HighestRate - (history.HighestRate - history.LowestRate) * 0.3M;

                            if (quote.Value <= history.LowestRate) // Lower than historical lowest rate
                            {
                                boardcast.HistoricalLowestRates.Add(quote.Key, new DailyExchangeRateBoardcastBenchmark(quote.Value, history.LowestRate, history.HighestRate));
                            }
                            else if (quote.Value <= lowestRate90) // Lower than 90% historical lowest rate
                            {
                                boardcast.HistoricalLowestRates90.Add(quote.Key, new DailyExchangeRateBoardcastBenchmark(quote.Value, history.LowestRate, history.HighestRate));
                            }
                            else if (quote.Value <= lowestRate80) // Lower than 80% historical lowest rate
                            {
                                boardcast.HistoricalLowestRates80.Add(quote.Key, new DailyExchangeRateBoardcastBenchmark(quote.Value, history.LowestRate, history.HighestRate));
                            }
                            else if (quote.Value <= lowestRate70) // Lower than 70% historical lowest rate
                            {
                                boardcast.HistoricalLowestRates70.Add(quote.Key, new DailyExchangeRateBoardcastBenchmark(quote.Value, history.LowestRate, history.HighestRate));
                            }

                            if (quote.Value >= history.HighestRate) // Higher than historical highest rate
                            {
                                boardcast.HistoricalHighestRates.Add(quote.Key, new DailyExchangeRateBoardcastBenchmark(quote.Value, history.LowestRate, history.HighestRate));
                            }
                            else if (quote.Value >= highestRate90) // Higher than 90% historical highest rate
                            {
                                boardcast.HistoricalHighestRates90.Add(quote.Key, new DailyExchangeRateBoardcastBenchmark(quote.Value, history.LowestRate, history.HighestRate));
                            }
                            else if (quote.Value >= highestRate80) // Higher than 80% historical highest rate
                            {
                                boardcast.HistoricalHighestRates80.Add(quote.Key, new DailyExchangeRateBoardcastBenchmark(quote.Value, history.LowestRate, history.HighestRate));
                            }
                            else if (quote.Value >= highestRate70) // Higher than 70% historical highest rate
                            {
                                boardcast.HistoricalHighestRates70.Add(quote.Key, new DailyExchangeRateBoardcastBenchmark(quote.Value, history.LowestRate, history.HighestRate));
                            }
                        }
                    }

                    if (translate == true)
                    {
                        // Translate currency subjects to Chinese.
                        string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, _configuration["ExchangeRates:Reference:CurrencySubjectFolder"], "currency-table-cn-zh.json");
                        var currencySubjects = _exchangeService.GetCurrencySubjects(filePath);

                        if (currencySubjects != null)
                        {
                            boardcast.HistoricalLowestRates.TranslateCurrencySubject(currencySubjects, "CNY");
                            boardcast.HistoricalLowestRates90.TranslateCurrencySubject(currencySubjects, "CNY");
                            boardcast.HistoricalLowestRates80.TranslateCurrencySubject(currencySubjects, "CNY");
                            boardcast.HistoricalLowestRates70.TranslateCurrencySubject(currencySubjects, "CNY");
                            boardcast.HistoricalHighestRates.TranslateCurrencySubject(currencySubjects, "CNY");
                            boardcast.HistoricalHighestRates90.TranslateCurrencySubject(currencySubjects, "CNY");
                            boardcast.HistoricalHighestRates80.TranslateCurrencySubject(currencySubjects, "CNY");
                            boardcast.HistoricalHighestRates70.TranslateCurrencySubject(currencySubjects, "CNY");
                        }
                    }
                }

                return new JsonResult(boardcast);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurs in API {nameof(GetDailyBoardcast)}: {ex.Message}");

                throw;
            }
        }

        private async Task<ExternalLiveRateApiStructure> RequestLiveExchangeRateAsync(string source, string? targets)
        {
            string liveUrl = _configuration["ExchangeRates:ResourceUrl:Live"];
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

            var result = await _httpRestService.GetResourcesAsync<ExternalLiveRateApiStructure>(httpClient, requestInfo);

            if (result.IsCompleted)
            {
                return result.Result;
            }
            else
            {
                _logger.LogError($"Failed in getting resource, error: {result.Error}");
                return null;
            }
        }
    }
}
