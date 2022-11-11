namespace RyanTechno.AzureApps.Domain.Currency;

public record CurrencySubscription
{
    public string Abbreviation { get; init; }

    public string DisplayName { get; init; }

    public bool Subscribed { get; set; }

    public decimal? MinMonitorRate { get; set; }

    public decimal? MaxMonitorRate { get; set; }

    public string ImageUrl { get; init; }
}
