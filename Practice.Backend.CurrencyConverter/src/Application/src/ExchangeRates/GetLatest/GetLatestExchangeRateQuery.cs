using MediatR;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.Shared;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;

public sealed class GetLatestExchangeRateQuery : IRequest<GetLatestExchangeRateQueryResponse>, IHaveCurrencies
{
    public required string BaseCurrency { get; set; }

    public required ExchangeRateProvider? Provider { get; set; }

    public IEnumerable<Currency> GetCurrencies()
    {
        yield return Currency.Create(BaseCurrency);
    }
}
