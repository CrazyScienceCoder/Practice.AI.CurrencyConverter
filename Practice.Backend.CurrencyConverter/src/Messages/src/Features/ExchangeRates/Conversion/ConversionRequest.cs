namespace Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;

public sealed record ConversionRequest : RequestBase
{
    public string BaseCurrency { get; init; } = null!;

    public string ToCurrency { get; init; } = null!;

    public decimal Amount { get; init; }
}
