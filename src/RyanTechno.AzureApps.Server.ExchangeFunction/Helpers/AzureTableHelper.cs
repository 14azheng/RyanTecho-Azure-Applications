using Azure;
using Azure.Data.Tables;
using RyanTechno.AzureApps.Server.ExchangeFunction.Models;
using System;
using System.Collections.Immutable;

namespace RyanTechno.AzureApps.Server.ExchangeFunction.Helpers
{
    public class AzureTableHelper
    {
        public static ExchangeServiceConfiguration GetExchangeServiceConfiguration(AzureStorageInfo storageInfo)
        {
            TableServiceClient serviceClient = GetTableServiceClient(storageInfo);

            var tableClient = serviceClient.GetTableClient("ServiceConfiguration");

            Pageable<ServiceConfigurationAzureTable> query = tableClient.Query<ServiceConfigurationAzureTable>(sc => sc.PartitionKey == "ExchangeService");

            return query.ToModel();
        }

        public static IImmutableList<CurrencyAzureTable> GetSubscribedCurrencyList(AzureStorageInfo storageInfo)
        {
            TableServiceClient serviceClient = GetTableServiceClient(storageInfo);

            var tableClient = serviceClient.GetTableClient("ExchangeCurrencySubscription");

            Pageable<CurrencyAzureTable> query = tableClient.Query<CurrencyAzureTable>();

            return query.ToImmutableArray();
        }

        private static TableServiceClient GetTableServiceClient(AzureStorageInfo storageInfo) => !string.IsNullOrEmpty(storageInfo.ConnectionString)
                ? new TableServiceClient(storageInfo.ConnectionString)
                : new TableServiceClient(new Uri(storageInfo.Endpoint), new TableSharedKeyCredential(storageInfo.AccountName, storageInfo.AccountKey));
    }

    public record AzureStorageInfo
    {
        public string ConnectionString { get; init; }

        public string Endpoint { get; init; }

        public string AccountName { get; init; }

        public string AccountKey { get; init; }
    }
}
