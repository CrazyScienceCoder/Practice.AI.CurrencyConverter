using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;

public sealed record HistoricalExchangeRateSnapshot
{
    public required Amount Amount { get; init; }

    public required Currency Base { get; init; }

    public required ExchangeDate StartDate { get; init; }

    public required ExchangeDate EndDate { get; init; }

    public Dictionary<ExchangeDate, Dictionary<Currency, Amount>> Rates { get; init; } = new();
}
