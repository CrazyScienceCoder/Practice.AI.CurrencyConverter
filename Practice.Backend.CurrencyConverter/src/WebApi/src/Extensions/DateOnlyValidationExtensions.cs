using FluentValidation;

namespace Practice.Backend.CurrencyConverter.WebApi.Extensions;

public static class DateOnlyValidationExtensions
{
    public static IRuleBuilderOptions<T, DateOnly?> MustBeValidDateOnly<T>(this IRuleBuilder<T, DateOnly?> ruleBuilder)
    {
        return ruleBuilder
            .NotNull()
            .WithMessage("{PropertyName} date is required.")
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("{PropertyName} date cannot be in the future.");
    }
}
