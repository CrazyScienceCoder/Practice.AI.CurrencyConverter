using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;

public sealed class ExchangeRateSnapshotProviderFactory(IEnumerable<IExchangeRateSnapshotProvider> providers)
    : IExchangeRateSnapshotProviderFactory
{
    public IExchangeRateSnapshotProvider GetProvider(ExchangeRateProvider provider)
    {
        var match = providers.FirstOrDefault(p => p.Provider == provider);

        if (match == null)
        {
            throw new InvalidOperationException($"No provider registered for {provider}");
        }

        return match;
    }
}
