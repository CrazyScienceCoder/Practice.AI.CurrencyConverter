using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Application.ExchangeRates.Shared;

public interface IHaveCurrencies
{
    IEnumerable<Currency> GetCurrencies();
}
