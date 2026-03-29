using FluentValidation;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;
using Practice.Backend.CurrencyConverter.WebApi.Extensions;

namespace Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Conversion;

public sealed class ConversionRequestValidator : AbstractValidator<ConversionRequest>
{
    private const int MinimumAmount = 1;

    public ConversionRequestValidator()
    {
        RuleFor(x => x.Provider)
            .MustBeValidProvider();

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(MinimumAmount);

        RuleFor(x => x.BaseCurrency)
            .MustBeValidCurrency();

        RuleFor(x => x.ToCurrency)
            .MustBeValidCurrency();
    }
}