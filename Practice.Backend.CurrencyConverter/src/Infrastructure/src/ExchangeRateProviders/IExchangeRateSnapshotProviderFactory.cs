using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;

public interface IExchangeRateSnapshotProviderFactory
{
    IExchangeRateSnapshotProvider GetProvider(ExchangeRateProvider provider);
}
