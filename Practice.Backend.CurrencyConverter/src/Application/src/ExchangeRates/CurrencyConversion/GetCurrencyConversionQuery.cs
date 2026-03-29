using MediatR;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.Shared;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Application.ExchangeRates.CurrencyConversion;

public sealed class GetCurrencyConversionQuery : IRequest<GetCurrencyConversionQueryResponse>, IHaveCurrencies
{
    public required string BaseCurrency { get; set; }

    public required string ToCurrency { get; set; }

    public required decimal Amount { get; set; }

    public required ExchangeRateProvider? Provider { get; set; }

    public IEnumerable<Currency> GetCurrencies()
    {
        yield return Currency.Create(BaseCurrency);
        yield return Currency.Create(ToCurrency);
    }
}
