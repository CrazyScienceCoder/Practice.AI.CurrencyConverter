using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Domain.ExchangeRates;

public sealed record ExchangeRate
{
    public required Amount Amount { get; init; }

    public required Currency Base { get; init; }

    public required ExchangeDate Date { get; init; }

    public Dictionary<Currency, Amount> Rates { get; init; } = new();
}
