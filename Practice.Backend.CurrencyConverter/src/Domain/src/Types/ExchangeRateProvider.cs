using Ardalis.SmartEnum;

namespace Practice.Backend.CurrencyConverter.Domain.Types;

public sealed class ExchangeRateProvider(string name, int value) : SmartEnum<ExchangeRateProvider>(name, value)
{
    public static readonly ExchangeRateProvider Frankfurter = new(nameof(Frankfurter), 1);
}
