namespace Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;

/// <summary>
/// Represents a historical exchange rate payload returned by the Currency Converter API.
/// Unlike the latest/conversion response, rates are keyed by date then by currency code.
/// </summary>
/// <param name="Amount">The base amount the rates are relative to.</param>
/// <param name="Base">The ISO 4217 base currency code.</param>
/// <param name="StartDate">The start date of the requested range.</param>
/// <param name="EndDate">The end date of the requested range.</param>
/// <param name="Rates">A map of date → (currency code → exchange rate).</param>
/// <param name="PageNumber">Current page number.</param>
/// <param name="HasMore">Whether there are more pages.</param>
/// <param name="TotalNumberOfPages">Total number of pages available.</param>
public sealed record HistoricalExchangeRateResponse(
    decimal Amount,
    string Base,
    DateOnly StartDate,
    DateOnly EndDate,
    IReadOnlyDictionary<DateOnly, IReadOnlyDictionary<string, decimal>> Rates,
    int PageNumber,
    bool HasMore,
    int TotalNumberOfPages);
