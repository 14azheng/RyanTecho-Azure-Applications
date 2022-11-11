using Azure;
using System;

namespace RyanTechno.AzureApps.Server.ExchangeFunction.Models
{
    public class CurrencyAzureTable : AzureTable
    {
        public string Source { get; set; }

        public string Target { get; set; }

        public double MinLevel { get; set; }

        public double MaxLevel { get; set; }

        public decimal LiveQuote { get; set; }
    }
}
