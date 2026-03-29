using ErrorOr;
using Practice.Backend.CurrencyConverter.Domain.CurrencyPolicy;
using Practice.Backend.CurrencyConverter.Domain.Types;
using CurrencyPolicySut = Practice.Backend.CurrencyConverter.Domain.CurrencyPolicy.CurrencyPolicy;

namespace Practice.Backend.CurrencyConverter.Domain.Tests.CurrencyPolicy;

public sealed class CurrencyPolicySpecifications
{
    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GBP")]
    [InlineData("JPY")]
    [InlineData("CHF")]
    public void EnsureAllowed_CurrencyIsAllowed_ReturnsSuccess(string code)
    {
        var sut = new CurrencyPolicySut();
        var currency = new Currency(code);

        var result = sut.EnsureAllowed(currency);

        result.IsError.Should().BeFalse();
    }

    [Theory]
    [InlineData("MXN")]
    [InlineData("PLN")]
    [InlineData("THB")]
    [InlineData("TRY")]
    public void EnsureAllowed_CurrencyIsForbidden_ReturnsForbiddenError(string code)
    {
        var sut = new CurrencyPolicySut();
        var currency = new Currency(code);

        var result = sut.EnsureAllowed(currency);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Forbidden);
        result.FirstError.Description.Should().Contain(code);
    }

    [Theory]
    [InlineData("mxn")]
    [InlineData("pln")]
    [InlineData("thb")]
    [InlineData("try")]
    public void EnsureAllowed_ForbiddenCurrencyIsLowercase_ReturnsForbiddenError(string code)
    {
        var sut = new CurrencyPolicySut();
        var currency = new Currency(code);

        var result = sut.EnsureAllowed(currency);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Forbidden);
    }

    [Fact]
    public void CurrencyPolicy_Type_ImplementsICurrencyPolicy()
    {
        var sut = new CurrencyPolicySut();

        sut.Should().BeAssignableTo<ICurrencyPolicy>();
    }
}
