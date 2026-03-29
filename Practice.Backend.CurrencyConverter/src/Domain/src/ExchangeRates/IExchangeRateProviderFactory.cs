using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Domain.ExchangeRates;

public interface IExchangeRateProviderFactory
{
    IExchangeRateProvider Create(ExchangeRateProvider provider);
}
