namespace Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;

public sealed record HistoricalExchangeRateResponse(
    decimal Amount,
    string Base,
    DateOnly StartDate,
    DateOnly EndDate,
    IReadOnlyDictionary<DateOnly, IReadOnlyDictionary<string, decimal>> Rates,
    int PageNumber,
    bool HasMore,
    int TotalNumberOfPages);
