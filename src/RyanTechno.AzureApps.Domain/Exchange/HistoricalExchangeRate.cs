using System.Collections.Immutable;

namespace RyanTechno.AzureApps.Domain.Exchange;

public record HistoricalExchangeRate
{
    public string Source { get; set; }

    public string Target { get; set; }

    public DateOnly Date { get; set; }

    public decimal Rate { get; set; }
}

public record HistoricalExchangeRateSummary
{
    public string Source { get; set; }

    public string Target { get; set; }

    public IImmutableDictionary<DateOnly, decimal> AllRates { get; set; }

    public decimal HighestRate { get; set; }

    public decimal LowestRate { get; set; }
}

public enum TopHighestPercent
{
    Highest100PCT = 0,
    Highest95PCT = 1,
    Highest90PCT = 2,
    Highest85PCT = 3,
    Highest80PCT = 4,
    Highest75PCT = 5,
    Highest70PCT = 6,
}

public enum TopLowestPercent
{
    Lowest100PCT = 0,
    Lowest95PCT = 1,
    Lowest90PCT = 2,
    Lowest85PCT = 3,
    Lowest80PCT = 4,
    Lowest75PCT = 5,
    Lowest70PCT = 6,
}
