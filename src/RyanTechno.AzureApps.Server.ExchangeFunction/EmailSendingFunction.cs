using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RyanTechno.AzureApps.Common.Interfaces.Network;
using RyanTechno.AzureApps.Common.Interfaces.Exchange;
using RyanTechno.AzureApps.Common.Interfaces.Infrastructure;
using RyanTechno.AzureApps.Common.Models;
using RyanTechno.AzureApps.Domain.Configuration;
using RyanTechno.AzureApps.Domain.Exchange;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using RyanTechno.AzureApps.Common.Models.Infrastructure;
using RyanTechno.AzureApps.Common.Models.Exchange;
using System.Net.Http;

namespace RyanTechno.AppService.ExchangeFunction
{
    public class EmailSendingFunction
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpRestService _httpRestService;
        private readonly IExchangeService _exchangeService;
        private readonly IEmailService _emailService;

        public EmailSendingFunction(
            IHttpRestService httpRestService,
            IExchangeService exchangeService,
            IEmailService emailService,
            IHttpClientFactory httpClientFactory)
        {
            this._httpRestService = httpRestService;
            this._exchangeService = exchangeService;
            this._emailService = emailService;
            this._httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Query live exchange rate timely and send notification to subscribers.
        /// </summary>
        /// <param name="myTimer"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        /// <remarks>Some timer trigger settings
        /// TimerTrigger("0 30 10 * * *") - at 10:30 am every day
        /// TimerTrigger("0 5 * * * *") - Once every hour of the day at minute 5 of each hour
        /// Time zone diff -8
        /// </remarks>
        [FunctionName("ExchangeRateEmailNotification")]
        public async Task Run([TimerTrigger("0 30 2 * * *")] TimerInfo myTimer, ILogger logger)
        {
            logger.LogInformation($"Exchange rate email notification trigger function executed at: {DateTime.Now}");

            try
            {
                await RetrieveDailyExchangeBoardcast(logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            logger.LogInformation($"Exchange rate email notification trigger function finished at: {DateTime.Now}");
        }

        private AzureStorageInfo GetAzureStorageInfo()
        {
            AzureStorageInfo storageInfo = new AzureStorageInfo
            {
                Endpoint = Environment.GetEnvironmentVariable("StorageServiceEndpoint"),
                AccountName = Environment.GetEnvironmentVariable("StorageServiceAccountName"),

                // Use Secret client to get secret from key vault, important: Managed Service Id (MSI) of this function app must be system-assigned, cannot be user-assigned.
                //AccountKey = await KeyVaultHelper.GetSecret("https://exchange-service-vault.vault.azure.net/", "storage-account-key"),

                // Configure storage account key in app setting referred to key vault,
                // see https://www.middleway.eu/accessing-azure-key-vault-from-azure-function/
                // or https://techmindfactory.com/Integrate-Key-Vault-Secrets-With-Azure-Functions/
                // Important: Managed Service Id (MSI) of this function app must be system-assigned, cannot be user-assigned.
                AccountKey = Environment.GetEnvironmentVariable("StorageServiceAccountKey"),
            };

            return storageInfo;
        }

        private async Task QueryLiveExchangeRateInfo(ILogger logger)
        {
            AzureStorageInfo storageInfo = GetAzureStorageInfo();

            logger.LogInformation("Storage info retrieved...");

            IImmutableList<CurrencySubscription> currencySubList = _exchangeService.GetSubscribedCurrencyList(storageInfo);

            logger.LogInformation("Currency subscription info retrieved...");

            ExchangeServiceConfiguration exchangeServiceConfiguration = _exchangeService.GetExchangeServiceConfiguration(storageInfo);

            logger.LogInformation("Exchange service configuration info retrieved...");

            HttpClient httpClient = this._httpClientFactory.CreateClient();

            var tokenTask = await _httpRestService.GetAccessTokenAsync(httpClient, new AuthenticationInfo
            {
                AcquireAccessTokenEndpoint = exchangeServiceConfiguration.AuthenticationServerEndpoint,
                ClientId = exchangeServiceConfiguration.AuthenticationClientId,
                ClientSecret = exchangeServiceConfiguration.AuthenticationClientSecret,
                Scope = exchangeServiceConfiguration.AuthenticationScope,
            });

            logger.LogInformation("Authentication token requesting...");

            if (tokenTask.IsCompleted)
            {
                string accessToken = tokenTask.Result;

                var exchangeTask = await _httpRestService.GetResourcesAsync<CurrencyApiStructure>(httpClient, new RestRequestInfo
                {
                    RequestEndpoint = $"{exchangeServiceConfiguration.LiveExchangeApiEndpoint}?source=CNY&targets={string.Join(',', currencySubList.Select(c => c.Target))}",
                    RequestHeaders = new Dictionary<string, string>
                    {
                        { "Authorization", "Bearer " + accessToken }
                    }
                });

                logger.LogInformation("Exchange live info requesting...");

                if (exchangeTask.IsCompleted)
                {
                    List<CurrencySubscription> exceedCurrencies = new();
                    List<CurrencySubscription> deficientCurrencies = new();

                    foreach (KeyValuePair<string, decimal> pair in exchangeTask.Result.Quotes)
                    {
                        decimal convertedRate = ConvertLiveQuote(pair.Key, pair.Value);

                        logger.LogInformation($"Reading currency exchange rate...{pair.Key}: {pair.Value} | converted rate: {convertedRate}");

                        var currency = currencySubList.FirstOrDefault(c => c.Source + c.Target == pair.Key);

                        if (currency is not null)
                        {
                            logger.LogInformation($"{pair.Key} Max Level: {currency.MaxMonitorRate} | Min Level: {currency.MinMonitorRate}");

                            if (convertedRate >= (decimal)currency.MaxMonitorRate)
                            {
                                exceedCurrencies.Add(currency);
                            }

                            if (convertedRate <= (decimal)currency.MinMonitorRate)
                            {
                                deficientCurrencies.Add(currency);
                            }

                            currency.LiveQuote = convertedRate;
                        }
                    }

                    if (exceedCurrencies.Any() || deficientCurrencies.Any())
                    {
                        var exceedStr = exceedCurrencies.Any() ? string.Join('|', exceedCurrencies.Select(c => c.Target)) : string.Empty;
                        var deficientStr = deficientCurrencies.Any() ? string.Join('|', deficientCurrencies.Select(c => c.Target)) : string.Empty;

                        logger.LogInformation($"Sending email notification...");
                        logger.LogInformation($"Exceed currencies: {exceedStr}");
                        logger.LogInformation($"Deficient currencies: {deficientStr}");

                        _emailService.SetSenderCredential(new OutlookSenderCredential
                        {
                            AccountName = Environment.GetEnvironmentVariable("EmailSenderAddress"),
                            Password = Environment.GetEnvironmentVariable("SmtpEmailPassword"),
                        });
                        _emailService.SendExchangeRateNotificationEmail(exceedCurrencies.ToImmutableArray(), deficientCurrencies.ToImmutableArray());
                    }
                    else
                    {
                        logger.LogInformation("No email notification need to be sent out...");
                    }
                }
                else
                {
                    logger.LogError($"Exchange live info request failed, reason: {exchangeTask.Error}");
                }
            }
            else
            {
                logger.LogError($"Authentication token request failed, reason: {tokenTask.Error}");
            }
        }

        private async Task RetrieveDailyExchangeBoardcast(ILogger logger)
        {
            AzureStorageInfo storageInfo = GetAzureStorageInfo();

            logger.LogInformation("Storage info retrieved...");

            /* subscribe all currencies.
            IImmutableList<CurrencySubscription> currencySubList = _exchangeService.GetSubscribedCurrencyList(storageInfo);

            logger.LogInformation("Currency subscription info retrieved...");
            */

            ExchangeServiceConfiguration exchangeServiceConfiguration = _exchangeService.GetExchangeServiceConfiguration(storageInfo);

            logger.LogInformation("Exchange service configuration info retrieved...");

            HttpClient httpClient = this._httpClientFactory.CreateClient();

            // Get access token from Identity Server.
            var tokenTask = await _httpRestService.GetAccessTokenAsync(httpClient, new AuthenticationInfo
            {
                AcquireAccessTokenEndpoint = exchangeServiceConfiguration.AuthenticationServerEndpoint,
                ClientId = exchangeServiceConfiguration.AuthenticationClientId,
                ClientSecret = exchangeServiceConfiguration.AuthenticationClientSecret,
                Scope = exchangeServiceConfiguration.AuthenticationScope,
            });

            logger.LogInformation("Authentication token requesting...");

            if (tokenTask.IsCompleted)
            {
                string accessToken = tokenTask.Result;

                // Get daily boardcast information from Web API.
                var exchangeTask = await _httpRestService.GetResourcesAsync<DailyExchangeRateBoardcastApiStructure>(httpClient, new RestRequestInfo
                {
                    //RequestEndpoint = $"{exchangeServiceConfiguration.DailyExchangeBoardcastApiEndpoint}?targets={string.Join(',', currencySubList.Select(c => c.Target))}",
                    RequestEndpoint = exchangeServiceConfiguration.DailyExchangeBoardcastApiEndpoint,
                    RequestHeaders = new Dictionary<string, string>
                    {
                        { "Authorization", "Bearer " + accessToken }
                    }
                });

                logger.LogInformation("Exchange boardcast info requesting...");

                if (exchangeTask.IsCompleted)
                {
                    DailyExchangeRateBoardcastApiStructure boardcast = exchangeTask.Result;

                    logger.LogInformation($"Exchange boardcast info retrieved...H100: {boardcast.HistoricalHighestRates.Count} | H90: {boardcast.HistoricalHighestRates90.Count} | H80: {boardcast.HistoricalHighestRates80.Count} | L100: {boardcast.HistoricalLowestRates.Count} | L90: {boardcast.HistoricalLowestRates90.Count} | L80: {boardcast.HistoricalLowestRates80.Count}");

                    logger.LogInformation($"Sending email notification...");

                    // Send email notification.
                    _emailService.SetSenderCredential(new OutlookSenderCredential
                    {
                        AccountName = Environment.GetEnvironmentVariable("EmailSenderAddress"),
                        Password = Environment.GetEnvironmentVariable("SmtpEmailPassword"),
                    });

                    logger.LogInformation($"Email notification body: {boardcast.ToEmailBody()}");

                    _emailService.SendExchangeDailyBoardcastEmail(boardcast);
                }
                else
                {
                    logger.LogError($"Exchange boardcast info request failed, reason: {exchangeTask.Error}");
                }
            }
            else
            {
                logger.LogError($"Authentication token request failed, reason: {tokenTask.Error}");
            }
        }

        private decimal ConvertLiveQuote(string currencyKey, decimal quote) => currencyKey switch
        {
            "CNYHKD" => 10 / quote,
            "CNYJPY" => 100 / quote,
            _ => 1 / quote,
        };
    }
}
