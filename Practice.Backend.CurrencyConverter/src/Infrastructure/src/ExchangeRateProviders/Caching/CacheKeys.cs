using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Caching;

public static class CacheKeys
{
    public const string Prefix = "fx:currency-converter";

    public static string Latest(Currency baseCurrency, ExchangeRateProvider provider)
        => $"{Prefix}:{provider.Name}:latest:{baseCurrency.Value}".ToLower();

    public static string Historical(
        Currency baseCurrency,
        ExchangeDate from,
        ExchangeDate to,
        ExchangeRateProvider provider)
        => $"{Prefix}:{provider.Name}:historical:{baseCurrency.Value}:{from.Value}:{to.Value}".ToLower();
}
