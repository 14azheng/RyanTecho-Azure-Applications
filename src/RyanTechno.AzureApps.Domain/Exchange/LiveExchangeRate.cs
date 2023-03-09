namespace RyanTechno.AzureApps.Domain.Exchange;

public record LiveExchangeRate
{
    public string Abbreviation { get; init; }

    public string DisplayName { get; init; }

    public decimal Rate { get; init; }

    public decimal OppositeRate { get; init; }

    public string ImageUrl { get; init; }
}
