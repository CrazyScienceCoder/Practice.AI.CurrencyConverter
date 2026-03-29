namespace Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetHistorical;

public sealed class GetHistoricalExchangeRateQueryResult
{
    public required decimal Amount { get; init; }

    public required string Base { get; init; }

    public required DateOnly StartDate { get; init; }

    public required DateOnly EndDate { get; init; }

    public Dictionary<DateOnly, Dictionary<string, decimal>> Rates { get; init; } = new();

    public required int PageNumber { get; set; }

    public required bool HasMore { get; set; }

    public required int TotalNumberOfPages { get; set; }
}
