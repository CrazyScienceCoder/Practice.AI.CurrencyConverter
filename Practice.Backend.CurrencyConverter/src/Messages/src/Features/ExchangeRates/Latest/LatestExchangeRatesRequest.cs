namespace Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;

/// <summary>
/// Request parameters for retrieving the latest exchange rates.
/// Maps to <c>GET /api/v{version}/exchange-rate/latest</c>.
/// </summary>
public sealed record LatestExchangeRatesRequest : RequestBase
{
    /// <summary>The ISO 4217 base currency code (e.g. "EUR").</summary>
    public string BaseCurrency { get; init; } = null!;
}
