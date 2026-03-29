using ErrorOr;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;

public interface IExchangeRateSnapshotProvider
{
    ExchangeRateProvider Provider { get; }

    Task<ErrorOr<ExchangeRateSnapshot>> GetLatestAsync(
        Currency baseCurrency,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<HistoricalExchangeRateSnapshot>> GetTimeSeriesAsync(
        Currency baseCurrency,
        ExchangeDate from,
        ExchangeDate to,
        CancellationToken cancellationToken = default);
}
