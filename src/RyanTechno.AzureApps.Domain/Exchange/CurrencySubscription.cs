namespace RyanTechno.AzureApps.Domain.Exchange;

public record CurrencySubscription
{
    public string Source { get; set; }

    public string Target { get; set; }

    public string DisplayName { get; init; }

    public bool Subscribed { get; set; }

    public decimal? MinMonitorRate { get; set; }

    public decimal? MaxMonitorRate { get; set; }

    public string ImageUrl { get; init; }

    public decimal? LiveQuote { get; set; }
}
