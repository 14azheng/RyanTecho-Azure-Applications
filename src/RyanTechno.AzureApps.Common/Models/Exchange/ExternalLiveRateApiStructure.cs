namespace RyanTechno.AzureApps.Common.Models.Exchange;

public record ExternalLiveRateApiStructure
{
    public bool Success { get; set; }

    public long TimeStamp { get; set; }

    public string Source { get; set; }

    public Dictionary<string, decimal> Quotes { get; set; }
}
