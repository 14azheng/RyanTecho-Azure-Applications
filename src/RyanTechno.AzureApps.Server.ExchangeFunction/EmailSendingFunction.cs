using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RyanTechno.AzureApps.Server.ExchangeFunction.Helpers;
using RyanTechno.AzureApps.Server.ExchangeFunction.Models;
using RyanTechno.AzureApps.Domain.Currency;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace RyanTechno.AppService.ExchangeFunction
{
    public class EmailSendingFunction
    {
        /// <summary>
        /// Query live exchange rate timely and send notification to subscribers.
        /// </summary>
        /// <param name="myTimer"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        /// <remarks>Some timer trigger settings
        /// TimerTrigger("0 30 10 * * *") - at 10:00 am every day
        /// TimerTrigger("0 5 * * * *") - Once every hour of the day at minute 5 of each hour
        /// </remarks>
        [FunctionName("ExchangeRateEmailNotification")]
        public async Task Run([TimerTrigger("0 30 10 * * *")] TimerInfo myTimer, ILogger logger)
        {
            logger.LogInformation($"Exchange rate email notification trigger function executed at: {DateTime.Now}");

            await QueryExchangeRateInfo(logger);

            logger.LogInformation($"Exchange rate email notification trigger function finished at: {DateTime.Now}");
        }

        private async Task QueryExchangeRateInfo(ILogger logger)
        {
            AzureStorageInfo storageInfo = new AzureStorageInfo
            {
                //ConnectionString = "DefaultEndpointsProtocol=https;AccountName=ryantechnostorage;AccountKey=y4z2N/6dWXUoPvBBphk25rZ6KX9AgYqxfoHIoTEYTmKTzQRpQ6jvggHINDPf+eQae8KLcEVZ/TJD+ASt+yaGWg==;EndpointSuffix=core.windows.net",
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

            IImmutableList<CurrencyAzureTable> currencySubList = AzureTableHelper.GetSubscribedCurrencyList(storageInfo);
            ExchangeServiceConfiguration exchangeServiceConfiguration = AzureTableHelper.GetExchangeServiceConfiguration(storageInfo);

            RestServiceProvider restServiceProvider = new RestServiceProvider(logger);

            var tokenTask = await restServiceProvider.GetAccessTokenAsync(new AuthenticationInfo
            {
                AcquireAccessTokenEndpoint = exchangeServiceConfiguration.AuthenticationServerEndpoint,
                ClientId = exchangeServiceConfiguration.AuthenticationClientId,
                ClientSecret = exchangeServiceConfiguration.AuthenticationClientSecret,
                Scope = exchangeServiceConfiguration.AuthenticationScope,
            });

            if (tokenTask.Success)
            {
                string accessToken = tokenTask.AccessToken;

                var currencyTask = await restServiceProvider.GetResourcesAsync<CurrencyApiStructure>(new RestRequestInfo
                {
                    RequestEndpoint = $"{exchangeServiceConfiguration.ExchangeApiEndpoint}?source=CNY&targets={string.Join(',', currencySubList.Select(c => c.Target))}",
                    RequestHeaders = new Dictionary<string, string>
                    {
                        { "Authorization", "Bearer " + accessToken }
                    }
                });

                if (currencyTask.Success)
                {
                    List<CurrencyAzureTable> exceedCurrencies = new();
                    List<CurrencyAzureTable> deficientCurrencies = new();

                    foreach (KeyValuePair<string, decimal> pair in currencyTask.Resource.Quotes)
                    {
                        decimal convertedRate = ConvertLiveQuote(pair.Key, pair.Value);

                        logger.LogInformation($"Reading currency exchange rate...{pair.Key}: {pair.Value} | converted rate: {convertedRate}");

                        var currency = currencySubList.FirstOrDefault(c => c.Source + c.Target == pair.Key);

                        if (currency is not null)
                        {
                            logger.LogInformation($"{pair.Key} Max Level: {currency.MaxLevel} | Min Level: {currency.MinLevel}");

                            if (convertedRate >= (decimal)currency.MaxLevel)
                            {
                                exceedCurrencies.Add(currency);
                            }

                            if (convertedRate <= (decimal)currency.MinLevel)
                            {
                                deficientCurrencies.Add(currency);
                            }

                            currency.LiveQuote = convertedRate;
                        }
                    }

                    if (exceedCurrencies.Any() || deficientCurrencies.Any())
                    {
                        OutlookEmailProvider outlookEmailProvider = new OutlookEmailProvider(exchangeServiceConfiguration.SmtpEmailAccount, Environment.GetEnvironmentVariable("SmtpEmailPassword"), logger);
                        outlookEmailProvider.SendExchangeRateNotificationEmail(exceedCurrencies.ToImmutableArray(), deficientCurrencies.ToImmutableArray());
                    }
                }
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
