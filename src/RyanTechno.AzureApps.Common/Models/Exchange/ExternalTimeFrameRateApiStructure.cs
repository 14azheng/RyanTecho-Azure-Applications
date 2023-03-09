using System.Text.Json.Serialization;

namespace RyanTechno.AzureApps.Common.Models.Exchange;

public record ExternalTimeFrameRateApiStructure
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("timeframe")]
    public bool Timeframe { get; set; }

    [JsonPropertyName("start_date")]
    public string StartDate { get; set; }

    [JsonPropertyName("end_date")]
    public string EndDate { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("quotes")]
    public Dictionary<string, Dictionary<string, decimal>> Quotes { get; set; }
}
