using ErrorOr;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Domain.CurrencyPolicy;

public interface ICurrencyPolicy
{
    ErrorOr<Success> EnsureAllowed(Currency currency);
}
