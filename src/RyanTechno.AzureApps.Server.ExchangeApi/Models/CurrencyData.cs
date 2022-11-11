namespace RyanTechno.AzureApps.Server.ExchangeApi.Models
{
    public record CurrencyData
    {
        public bool Success { get; set; }

        public long TimeStamp { get; set; }

        public string Source { get; set; }

        public Dictionary<string, decimal> Quotes { get; set; }
    }
}
