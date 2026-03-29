using Practice.Backend.CurrencyConverter.Application.Shared.Extensions;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Application.Tests.Shared.Extensions;

public sealed class CurrencyExtensionsSpecifications
{
    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GBP")]
    [InlineData("JPY")]
    [InlineData("CHF")]
    public void IsSupportedCurrency_AllowedCurrency_ReturnsTrue(string currencyCode)
    {
        var currency = new Currency(currencyCode);

        var result = currency.IsSupportedCurrency();

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("MXN")]
    [InlineData("PLN")]
    [InlineData("THB")]
    [InlineData("TRY")]
    public void IsSupportedCurrency_ForbiddenCurrency_ReturnsFalse(string currencyCode)
    {
        var currency = new Currency(currencyCode);

        var result = currency.IsSupportedCurrency();

        result.Should().BeFalse();
    }
}
