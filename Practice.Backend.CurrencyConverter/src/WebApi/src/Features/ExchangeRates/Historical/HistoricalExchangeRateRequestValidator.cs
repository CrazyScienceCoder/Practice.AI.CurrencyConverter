using FluentValidation;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;
using Practice.Backend.CurrencyConverter.WebApi.Extensions;

namespace Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Historical;

public sealed class HistoricalExchangeRateRequestValidator : AbstractValidator<HistoricalExchangeRateRequest>
{
    private const int MinimumPositiveValue = 1;
    private const int MaximumNumberOfDaysPerPage = 30;

    public HistoricalExchangeRateRequestValidator()
    {
        RuleFor(x => x.BaseCurrency)
            .MustBeValidCurrency();

        RuleFor(x => x.From)
            .MustBeValidDateOnly();

        RuleFor(x => x.To)
            .MustBeValidDateOnly();

        RuleFor(x => x.Provider)
            .MustBeValidProvider();

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(MinimumPositiveValue)
            .When(x => x.PageNumber.HasValue)
            .WithMessage($"{{PropertyName}} must be greater than or equal to {MinimumPositiveValue}.");

        RuleFor(x => x.DaysPerPage)
            .InclusiveBetween(MinimumPositiveValue, MaximumNumberOfDaysPerPage)
            .When(x => x.DaysPerPage.HasValue)
            .WithMessage($"{{PropertyName}} must be between {MinimumPositiveValue} and {MaximumNumberOfDaysPerPage}.");
    }
}
