using Azure;
using Azure.Data.Tables;
using RyanTechno.AzureApps.Common.Interfaces.Exchange;
using RyanTechno.AzureApps.Common.Models;
using RyanTechno.AzureApps.Domain.Configuration;
using RyanTechno.AzureApps.Domain.Exchange;
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
    }
}
