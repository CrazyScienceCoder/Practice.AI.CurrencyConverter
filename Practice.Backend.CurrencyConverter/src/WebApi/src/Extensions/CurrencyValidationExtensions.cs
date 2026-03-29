using System.Globalization;
using FluentValidation;

namespace Practice.Backend.CurrencyConverter.WebApi.Extensions;

public static class CurrencyValidationExtensions
{
    private static readonly HashSet<string> IsoCurrencyCodes = CultureInfo
        .GetCultures(CultureTypes.SpecificCultures)
        .Select(ci => new RegionInfo(ci.Name).ISOCurrencySymbol)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    public static IRuleBuilderOptions<T, string> MustBeValidCurrency<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(code => IsoCurrencyCodes.Contains(code))
            .WithMessage("{PropertyName} must be a valid ISO 4217 code.");
    }
}
