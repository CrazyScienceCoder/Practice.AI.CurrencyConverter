using ErrorOr;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Domain.ExchangeRates;

public interface IExchangeRateProvider
{
    ExchangeRateProvider Provider { get; }

    Task<ErrorOr<ExchangeRate>> ConvertAsync(Currency baseCurrency
        , Currency toCurrency
        , Amount amount
        , CancellationToken cancellationToken = default);

    Task<ErrorOr<ExchangeRate>> GetLatestExchangeRateAsync(Currency baseCurrency
        , CancellationToken cancellationToken = default);

    Task<ErrorOr<HistoricalExchangeRate>> GetHistoricalExchangeRateAsync(Currency baseCurrency
        , ExchangeDate from
        , ExchangeDate to
        , CancellationToken cancellationToken = default);
}
