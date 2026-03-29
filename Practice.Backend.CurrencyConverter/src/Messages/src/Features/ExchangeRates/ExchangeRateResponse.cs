namespace Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates;

/// <summary>
/// Represents an exchange rate payload returned by the Currency Converter API.
/// All three endpoints (latest, historical, conversion) share this response shape.
/// </summary>
/// <param name="Amount">The base amount the rates are relative to.</param>
/// <param name="Base">The ISO 4217 base currency code.</param>
/// <param name="Date">The date the rates apply to.</param>
/// <param name="Rates">A map of target currency codes to their exchange rates.</param>
public sealed record ExchangeRateResponse(
    decimal Amount,
    string Base,
    DateOnly Date,
    IReadOnlyDictionary<string, decimal> Rates);
