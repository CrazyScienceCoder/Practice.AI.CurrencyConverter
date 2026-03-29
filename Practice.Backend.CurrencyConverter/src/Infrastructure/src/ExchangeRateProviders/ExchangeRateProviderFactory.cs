using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;

public sealed class ExchangeRateProviderFactory(IEnumerable<IExchangeRateProvider> providers) : IExchangeRateProviderFactory
{
    public IExchangeRateProvider Create(ExchangeRateProvider provider)
    {
        var exchangeRateProvider = providers.FirstOrDefault(p => p.Provider == provider);

        if (exchangeRateProvider is null)
        {
            throw new InvalidOperationException($"No ExchangeRateProvider for {provider.Name}");
        }

        return exchangeRateProvider;
    }
}
