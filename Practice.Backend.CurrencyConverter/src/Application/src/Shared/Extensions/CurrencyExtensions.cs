using Practice.Backend.CurrencyConverter.Domain.CurrencyPolicy;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Application.Shared.Extensions;

public static class CurrencyExtensions
{
    public static bool IsSupportedCurrency(this Currency currency)
        => !new CurrencyPolicy().EnsureAllowed(currency).IsError;
}
