using System.Text.Json.Serialization;
using System.Linq;

namespace RyanTechno.AzureApps.Common.Models.Exchange;

public record DailyExchangeRateBoardcastApiStructure
{
    [JsonPropertyName("historical_highest_rates")]
    public IDictionary<string, DailyExchangeRateBoardcastBenchmark> HistoricalHighestRates { get; set; } = new Dictionary<string, DailyExchangeRateBoardcastBenchmark>();

    [JsonPropertyName("historical_highest_rates_90")]
    public IDictionary<string, DailyExchangeRateBoardcastBenchmark> HistoricalHighestRates90 { get; set; } = new Dictionary<string, DailyExchangeRateBoardcastBenchmark>();

    [JsonPropertyName("historical_highest_rates_80")]
    public IDictionary<string, DailyExchangeRateBoardcastBenchmark> HistoricalHighestRates80 { get; set; } = new Dictionary<string, DailyExchangeRateBoardcastBenchmark>();

    [JsonPropertyName("historical_highest_rates_70")]
    public IDictionary<string, DailyExchangeRateBoardcastBenchmark> HistoricalHighestRates70 { get; set; } = new Dictionary<string, DailyExchangeRateBoardcastBenchmark>();

    [JsonPropertyName("historical_lowest_rates")]
    public IDictionary<string, DailyExchangeRateBoardcastBenchmark> HistoricalLowestRates { get; set; } = new Dictionary<string, DailyExchangeRateBoardcastBenchmark>();

    [JsonPropertyName("historical_lowest_rates_90")]
    public IDictionary<string, DailyExchangeRateBoardcastBenchmark> HistoricalLowestRates90 { get; set; } = new Dictionary<string, DailyExchangeRateBoardcastBenchmark>();

    [JsonPropertyName("historical_lowest_rates_80")]
    public IDictionary<string, DailyExchangeRateBoardcastBenchmark> HistoricalLowestRates80 { get; set; } = new Dictionary<string, DailyExchangeRateBoardcastBenchmark>();

    [JsonPropertyName("historical_lowest_rates_70")]
    public IDictionary<string, DailyExchangeRateBoardcastBenchmark> HistoricalLowestRates70 { get; set; } = new Dictionary<string, DailyExchangeRateBoardcastBenchmark>();

    public string ToEmailBody() => $"<html><body><b>历史最低 (建议卖出)</b><br/><br/>{string.Join("<br/>", (HistoricalLowestRates.Count == 0 ? new string[] { "N/A" } : HistoricalLowestRates.OrderByDescending(l => l.Value.LowerPCT).Select(l => OutputBoardcastMessage(l, false))))}" +
                        $"<br/><br/><b>历史最低 - 90%</b><br/><br/>{string.Join("<br/>", (HistoricalLowestRates90.Count == 0 ? new string[] { "N/A" } : HistoricalLowestRates90.OrderByDescending(l => l.Value.LowerPCT).Select(l => OutputBoardcastMessage(l, false))))}" +
                        $"<br/><br/><b>历史最低 - 80%</b><br/><br/>{string.Join("<br/>", (HistoricalLowestRates80.Count == 0 ? new string[] { "N/A" } : HistoricalLowestRates80.OrderByDescending(l => l.Value.LowerPCT).Select(l => OutputBoardcastMessage(l, false))))}" +
                        $"<br/><br/><b>历史最低 - 70%</b><br/><br/>{string.Join("<br/>", (HistoricalLowestRates70.Count == 0 ? new string[] { "N/A" } : HistoricalLowestRates70.OrderByDescending(l => l.Value.LowerPCT).Select(l => OutputBoardcastMessage(l, false))))}" +
                        $"<br/><br/><b>历史最高 (建议买入)</b><br/><br/>{string.Join("<br/>", (HistoricalHighestRates.Count == 0 ? new string[] { "N/A" } : HistoricalHighestRates.OrderByDescending(h => h.Value.HigherPCT).Select(h => OutputBoardcastMessage(h, true))))}" +
                        $"<br/><br/><b>历史最高 - 90%</b><br/><br/>{string.Join("<br/>", (HistoricalHighestRates90.Count == 0 ? new string[] { "N/A" } : HistoricalHighestRates90.OrderByDescending(h => h.Value.HigherPCT).Select(h => OutputBoardcastMessage(h, true))))}" +
                        $"<br/><br/><b>历史最高 - 80%</b><br/><br/>{string.Join("<br/>", (HistoricalHighestRates80.Count == 0 ? new string[] { "N/A" } : HistoricalHighestRates80.OrderByDescending(h => h.Value.HigherPCT).Select(h => OutputBoardcastMessage(h, true))))}" +
                        $"<br/><br/><b>历史最高 - 70%</b><br/><br/>{string.Join("<br/>", (HistoricalHighestRates70.Count == 0 ? new string[] { "N/A" } : HistoricalHighestRates70.OrderByDescending(h => h.Value.HigherPCT).Select(h => OutputBoardcastMessage(h, true))))}" +
                        "</body></html>";

    private string OutputBoardcastMessage(KeyValuePair<string, DailyExchangeRateBoardcastBenchmark> rateInfo, bool highOrLow) => 
        $"{rateInfo.Key}: {rateInfo.Value.LiveRate.ToString("0.000")} ({(highOrLow ? rateInfo.Value.HigherPCT : rateInfo.Value.LowerPCT)}%)";
}

public record DailyExchangeRateBoardcastBenchmark
{
    [JsonPropertyName("live_rate")]
    public decimal LiveRate { get; set; }

    [JsonPropertyName("lowest_rate")]
    public decimal LowestRate { get; set; }

    [JsonPropertyName("highest_rate")]
    public decimal HightestRate { get; set; }

    [JsonIgnore]
    public decimal LowerPCT => Math.Round((HightestRate - LiveRate) / (HightestRate - LowestRate) * 100, 1);

    [JsonIgnore]
    public decimal HigherPCT => Math.Round((LiveRate - LowestRate) / (HightestRate - LowestRate) * 100, 1);

    public DailyExchangeRateBoardcastBenchmark()
    {
        
    }

    public DailyExchangeRateBoardcastBenchmark(decimal live, decimal lowest, decimal highest)
    {
        LiveRate = live;
        LowestRate = lowest;
        HightestRate = highest;
    }
}
