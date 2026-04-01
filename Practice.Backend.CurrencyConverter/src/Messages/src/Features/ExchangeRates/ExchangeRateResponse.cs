namespace Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates;

public sealed record ExchangeRateResponse(
    decimal Amount,
    string Base,
    DateOnly Date,
    IReadOnlyDictionary<string, decimal> Rates);
