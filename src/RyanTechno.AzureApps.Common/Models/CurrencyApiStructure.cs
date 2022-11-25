namespace RyanTechno.AzureApps.Common.Models;

public record CurrencyApiStructure
{
    public bool Success { get; set; }

    public long TimeStamp { get; set; }

    public string Source { get; set; }

    public Dictionary<string, decimal> Quotes { get; set; }
}
