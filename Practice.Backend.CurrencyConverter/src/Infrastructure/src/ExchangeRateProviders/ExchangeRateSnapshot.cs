using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;

public sealed record ExchangeRateSnapshot
{
    public required Currency Base { get; init; }

    public required ExchangeDate Date { get; init; }

    public Dictionary<Currency, Amount> Rates { get; init; } = new();
}
