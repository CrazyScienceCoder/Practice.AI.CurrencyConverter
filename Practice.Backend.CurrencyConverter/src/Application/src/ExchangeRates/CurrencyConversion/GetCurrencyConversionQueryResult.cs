namespace Practice.Backend.CurrencyConverter.Application.ExchangeRates.CurrencyConversion;

public sealed class GetCurrencyConversionQueryResult
{
    public required decimal Amount { get; init; }

    public required string Base { get; init; }

    public required DateOnly Date { get; init; }

    public Dictionary<string, decimal> Rates { get; init; } = new();
}
