using FluentValidation;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;
using Practice.Backend.CurrencyConverter.WebApi.Extensions;

namespace Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Latest;

public sealed class LatestExchangeRatesRequestValidator : AbstractValidator<LatestExchangeRatesRequest>
{
    public LatestExchangeRatesRequestValidator()
    {
        RuleFor(x => x.Provider)
            .MustBeValidProvider();

        RuleFor(x => x.BaseCurrency)
            .MustBeValidCurrency();
    }
}