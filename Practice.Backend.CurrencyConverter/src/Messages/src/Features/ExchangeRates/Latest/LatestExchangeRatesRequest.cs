namespace Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;

public sealed record LatestExchangeRatesRequest : RequestBase
{
    public string BaseCurrency { get; init; } = null!;
}
