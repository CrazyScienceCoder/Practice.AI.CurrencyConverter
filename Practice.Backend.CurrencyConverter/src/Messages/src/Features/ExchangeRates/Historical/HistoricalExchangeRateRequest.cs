namespace Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;

public sealed record HistoricalExchangeRateRequest : RequestBase
{
    public string BaseCurrency { get; init; } = null!;

    public DateOnly? From { get; init; }

    public DateOnly? To { get; init; }

    public int? PageNumber { get; init; }

    public int? DaysPerPage { get; init; }
}
