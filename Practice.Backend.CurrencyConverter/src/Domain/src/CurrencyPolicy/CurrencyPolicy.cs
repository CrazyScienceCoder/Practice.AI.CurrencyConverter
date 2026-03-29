using ErrorOr;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Domain.CurrencyPolicy;

public sealed class CurrencyPolicy : ICurrencyPolicy
{
    public ErrorOr<Success> EnsureAllowed(Currency currency)
    {
        var isForbidden = ForbiddenCurrencies.List
            .Any(i => i.Code.Equals(currency.Value, StringComparison.OrdinalIgnoreCase));

        if (isForbidden)
        {
            return Error.Forbidden(description: $"Currency: {currency.Value} is not allowed.");
        }

        return Result.Success;
    }
}
