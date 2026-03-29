namespace Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates;

public abstract record RequestBase
{
    public string? Provider { get; init; }
}
