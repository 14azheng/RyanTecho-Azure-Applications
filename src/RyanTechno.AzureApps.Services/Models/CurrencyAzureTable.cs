using RyanTechno.AzureApps.Domain.Exchange;

namespace RyanTechno.AzureApps.Services.Models
{
    internal class CurrencyAzureTable : AzureTable
    {
        public string Source { get; set; }

        public string Target { get; set; }

        public double MinLevel { get; set; }

        public double MaxLevel { get; set; }

        public decimal LiveQuote { get; set; }
    }

    internal static class CurrencyAzureTableExtension
    {
        public static CurrencySubscription ToModel(this CurrencyAzureTable currencyAzureTable)
        {
            return new CurrencySubscription
            {
                Source = currencyAzureTable.Source,
                Target = currencyAzureTable.Target,
                MaxMonitorRate = (decimal)currencyAzureTable.MaxLevel,
                MinMonitorRate = (decimal)currencyAzureTable.MinLevel,
            };
        }
    }
}
