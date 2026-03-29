using ErrorOr;
using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Mappers;

namespace Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Frankfurter;

public sealed class FrankfurterExchangeRateProvider : IExchangeRateProvider
{
    private readonly IExchangeRateSnapshotProvider _snapshotProvider;

    public FrankfurterExchangeRateProvider(IExchangeRateSnapshotProviderFactory factory)
    {
        _snapshotProvider = factory.GetProvider(Provider);
    }

    public ExchangeRateProvider Provider => ExchangeRateProvider.Frankfurter;

    public async Task<ErrorOr<ExchangeRate>> ConvertAsync(Currency baseCurrency
        , Currency toCurrency
        , Amount amount
        , CancellationToken cancellationToken = default)
    {
        var rate = await _snapshotProvider.GetLatestAsync(baseCurrency, cancellationToken);

        if (rate.IsError)
        {
            return rate.FirstError;
        }

        return rate.Value.ToExchangeRate(amount, toCurrency);
    }

    public async Task<ErrorOr<ExchangeRate>> GetLatestExchangeRateAsync(Currency baseCurrency
        , CancellationToken cancellationToken = default)
    {
        var rate = await _snapshotProvider.GetLatestAsync(baseCurrency, cancellationToken);

        if (rate.IsError)
        {
            return rate.FirstError;
        }

        return rate.Value.ToExchangeRate(amount: 1);
    }

    public async Task<ErrorOr<HistoricalExchangeRate>> GetHistoricalExchangeRateAsync(Currency baseCurrency
        , ExchangeDate from
        , ExchangeDate to
        , CancellationToken cancellationToken = default)
    {
        var rate = await _snapshotProvider.GetTimeSeriesAsync(baseCurrency, from, to, cancellationToken);

        if (rate.IsError)
        {
            return rate.FirstError;
        }

        return rate.Value.ToHistoricalExchangeRate();
    }
}
