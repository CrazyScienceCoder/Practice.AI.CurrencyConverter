using MediatR;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.Shared;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetHistorical;

public sealed class GetHistoricalExchangeRateQuery : IRequest<GetHistoricalExchangeRateQueryResponse>, IHaveCurrencies
{
    public required string BaseCurrency { get; set; }

    public DateOnly From { get; set; }

    public DateOnly To { get; set; }

    public required ExchangeRateProvider? Provider { get; set; }

    public int? PageNumber { get; set; }

    public int? DaysPerPage { get; set; }

    public IEnumerable<Currency> GetCurrencies()
    {
        yield return Currency.Create(BaseCurrency);
    }
}
