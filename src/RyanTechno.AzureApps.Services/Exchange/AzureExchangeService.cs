using Azure;
using Azure.Data.Tables;
using RyanTechno.AzureApps.Common.Interfaces.Exchange;
using RyanTechno.AzureApps.Common.Models;
using RyanTechno.AzureApps.Common.Models.Exchange;
using RyanTechno.AzureApps.Domain.Configuration;
using RyanTechno.AzureApps.Domain.Exchange;
using RyanTechno.AzureApps.Infrastructure.Helpers;
using RyanTechno.AzureApps.Services.Models;
using System.Collections.Immutable;

namespace RyanTechno.AzureApps.Services.Exchange
{
    public class AzureExchangeService : IExchangeService
    {
        public ExchangeServiceConfiguration GetExchangeServiceConfiguration(AzureStorageInfo storageInfo)
        {
            TableServiceClient serviceClient = GetTableServiceClient(storageInfo);

            var tableClient = serviceClient.GetTableClient("ServiceConfiguration");

            Pageable<ServiceConfigurationAzureTable> query = tableClient.Query<ServiceConfigurationAzureTable>(sc => sc.PartitionKey == "ExchangeService");

            return query.ToModel();
        }

        public IImmutableList<CurrencySubscription> GetSubscribedCurrencyList(AzureStorageInfo storageInfo)
        {
            TableServiceClient serviceClient = GetTableServiceClient(storageInfo);

            var tableClient = serviceClient.GetTableClient("ExchangeCurrencySubscription");

            Pageable<CurrencyAzureTable> query = tableClient.Query<CurrencyAzureTable>();

            return query.Select(c => c.ToModel()).ToImmutableArray();
        }

        public Task<ServiceResult<List<CurrencySubscription>>> LoadExchangeSubscriptionsAsync(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> SaveExchangeSubscriptionsAsync(IEnumerable<CurrencySubscription> subscriptions, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        private static TableServiceClient GetTableServiceClient(AzureStorageInfo storageInfo) => !string.IsNullOrEmpty(storageInfo.ConnectionString)
                ? new TableServiceClient(storageInfo.ConnectionString)
                : new TableServiceClient(new Uri(storageInfo.Endpoint), new TableSharedKeyCredential(storageInfo.AccountName, storageInfo.AccountKey));

        public IImmutableDictionary<string, SortedDictionary<DateOnly, HistoricalExchangeRate>> GetHistoricalExchangeRatesFromFiles(string folder)
        {
            // Read historical rates from archived files.
            string[] jsonFiles = Directory.GetFiles(folder);
            List<ExternalTimeFrameRateApiStructure> historicalApiExchangeRates = new();

            foreach (string jsonFile in jsonFiles)
            {
                ExternalTimeFrameRateApiStructure historicalApiExchangeRate = IOHelper.DeserializeJsonFile<ExternalTimeFrameRateApiStructure>(jsonFile);
                historicalApiExchangeRates.Add(historicalApiExchangeRate);
            }

            // Convert to historical rate models
            Dictionary<string, SortedDictionary<DateOnly, HistoricalExchangeRate>> historicalExchangeRates = new();
            HistoricalExchangeRate historicalExchangeRate = null;
            string source = "CNY";

            foreach (ExternalTimeFrameRateApiStructure historicalApi in historicalApiExchangeRates)
            {
                foreach(string date in historicalApi.Quotes.Keys)
                {
                    Dictionary<string, decimal> quotes = historicalApi.Quotes[date];
                    
                    foreach (KeyValuePair<string, decimal> quote in quotes)
                    {
                        string target = quote.Key;
                        decimal rate = quote.Value;

                        historicalExchangeRate = new HistoricalExchangeRate
                        {
                            Date = DateOnly.Parse(date),
                            Source = source,
                            Target = target,
                            Rate = rate,
                        };

                        if (historicalExchangeRates.ContainsKey(target))
                        {
                            historicalExchangeRates[target].Add(historicalExchangeRate.Date, historicalExchangeRate);
                        }
                        else
                        {
                            SortedDictionary<DateOnly, HistoricalExchangeRate> historicalExchanges = new()
                            {
                                { historicalExchangeRate.Date, historicalExchangeRate }
                            };
                            historicalExchangeRates.Add(target, historicalExchanges);
                        }
                    }
                }
            }
            
            return historicalExchangeRates.ToImmutableDictionary();
        }

        public IImmutableDictionary<string, HistoricalExchangeRateSummary> SummarizeHistoricalExchangeRates(IImmutableDictionary<string, SortedDictionary<DateOnly, HistoricalExchangeRate>> rawData)
        {
            return rawData?.Select(raw => new HistoricalExchangeRateSummary
            {
                Source = "CNY",
                Target = raw.Key,
                AllRates = raw.Value.ToImmutableDictionary(r => r.Key, r => r.Value.Rate),
                HighestRate = raw.Value.Max(r => r.Value.Rate),
                LowestRate = raw.Value.Min(r => r.Value.Rate),
            }).ToImmutableDictionary(s => s.Target);
        }
    }
}
