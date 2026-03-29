using FluentValidation;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.WebApi.Extensions;

public static class ProviderValidationExtensions
{
    private static readonly string Providers = string.Join(", ", ExchangeRateProvider.List.Select(p => p.Name));

    public static IRuleBuilderOptions<T, string?> MustBeValidProvider<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(value => value is null || ExchangeRateProvider.TryFromName(value, ignoreCase: true, out _))
            .WithMessage($"{{PropertyName}} must be one of: [{Providers}]");
    }
}
