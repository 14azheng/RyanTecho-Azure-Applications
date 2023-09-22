using System.Text.Json.Serialization;

namespace RyanTechno.AzureApps.Common.Models.Stock
{
    public class DailyStockApiStructure
    {
        [JsonPropertyName("Meta Data")]
        public DailyStockMetadata? Metadata { get; set; }

        [JsonPropertyName("Time Series (Daily)")]
        public Dictionary<DateTime, DailyStockValue>? TimeSeries { get; set; }
    }

    public class DailyStockMetadata
    {
        [JsonPropertyName("1. Information")]
        public string? Information { get; set; }

        [JsonPropertyName("2. Symbol")]
        public string? Symbol { get; set; }

        [JsonPropertyName("3. Last Refreshed")]
        public DateTime? LastRefreshed { get; set; }

        [JsonPropertyName("4. Output Size")]
        public string? OutputSize { get; set; }

        [JsonPropertyName("5. Time Zone")]
        public string? TimeZone { get; set; }
    }

    public class DailyStockValue
    {
        [JsonPropertyName("1. open")]
        public double Open { get; set; }

        [JsonPropertyName("2. high")]
        public double High { get; set; }

        [JsonPropertyName("3. low")]
        public double Low { get; set; }

        [JsonPropertyName("4. close")]
        public double Close { get; set; }

        [JsonPropertyName("5. volume")]
        public double Volume { get; set; }
    }
}
