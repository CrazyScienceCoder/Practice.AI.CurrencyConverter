namespace Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;

/// <summary>
/// Request parameters for retrieving historical exchange rates.
/// Maps to <c>GET /api/v{version}/exchange-rate/historical</c>.
/// </summary>
public sealed record HistoricalExchangeRateRequest : RequestBase
{
    /// <summary>The ISO 4217 base currency code (e.g. "EUR").</summary>
    public string BaseCurrency { get; init; } = null!;

    /// <summary>Start of the date range (inclusive).</summary>
    public DateOnly? From { get; init; }

    /// <summary>End of the date range (inclusive).</summary>
    public DateOnly? To { get; init; }

    /// <summary>Optional 1-based page number for server-side pagination.</summary>
    public int? PageNumber { get; init; }

    /// <summary>Optional number of days per page (max 30).</summary>
    public int? DaysPerPage { get; init; }
}
