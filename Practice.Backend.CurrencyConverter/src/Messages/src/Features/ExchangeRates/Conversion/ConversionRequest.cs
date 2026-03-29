namespace Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;

/// <summary>
/// Request parameters for converting an amount between currencies.
/// Maps to <c>GET /api/v{version}/exchange-rate/conversion</c>.
/// </summary>
public sealed record ConversionRequest : RequestBase
{
    /// <summary>The ISO 4217 source currency code (e.g. "EUR").</summary>
    public string BaseCurrency { get; init; } = null!;

    /// <summary>The ISO 4217 target currency code (e.g. "USD").</summary>
    public string ToCurrency { get; init; } = null!;

    /// <summary>The amount to convert.</summary>
    public decimal Amount { get; init; }
}
