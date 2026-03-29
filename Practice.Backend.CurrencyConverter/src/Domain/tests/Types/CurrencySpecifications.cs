using Practice.Backend.CurrencyConverter.Domain.Exceptions;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Domain.Tests.Types;

public sealed class CurrencySpecifications
{
    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GB")]
    [InlineData("A")]
    public void Constructor_CodeIsValid_CreatesCurrency(string code)
    {
        var currency = new Currency(code);

        currency.Value.Should().Be(code);
    }

    [Fact]
    public void Constructor_CodeIsNull_ThrowsInvalidCurrencyCodeException()
    {
        var act = () => new Currency(null!);

        act.Should().ThrowExactly<InvalidCurrencyCodeException>()
            .Which.ParamName.Should().Be("currencyCode");
    }

    [Fact]
    public void Constructor_CodeIsEmpty_ThrowsInvalidCurrencyCodeException()
    {
        var act = () => new Currency(string.Empty);

        act.Should().ThrowExactly<InvalidCurrencyCodeException>()
            .Which.ParamName.Should().Be("currencyCode");
    }

    [Theory]
    [InlineData("USDD")]
    [InlineData("TOOLONG")]
    public void Constructor_CodeExceedsMaxLength_ThrowsInvalidCurrencyCodeException(string code)
    {
        var act = () => new Currency(code);

        act.Should().ThrowExactly<InvalidCurrencyCodeException>()
            .Which.ParamName.Should().Be("currencyCode");
    }

    [Fact]
    public void Constructor_CodeIsExactlyMaxLength_CreatesCurrency()
    {
        var code = new string('X', Currency.MaxLength);

        var currency = new Currency(code);

        currency.Value.Should().Be(code);
    }

    [Fact]
    public void MaxLength_Always_IsThree()
    {
        Currency.MaxLength.Should().Be(3);
    }

    [Fact]
    public void Create_CodeIsValid_ReturnsCurrency()
    {
        var currency = Currency.Create("USD");

        currency.Value.Should().Be("USD");
    }

    [Fact]
    public void ImplicitOperator_FromString_CreatesCurrency()
    {
        Currency currency = "EUR";

        currency.Value.Should().Be("EUR");
    }

    [Fact]
    public void ImplicitOperator_ToString_ReturnsValue()
    {
        var currency = new Currency("GBP");

        string result = currency;

        result.Should().Be("GBP");
    }

    [Fact]
    public void Equals_SameCode_ReturnsTrue()
    {
        var c1 = new Currency("USD");
        var c2 = new Currency("USD");

        c1.Should().Be(c2);
    }

    [Fact]
    public void Equals_DifferentCodes_ReturnsFalse()
    {
        var c1 = new Currency("USD");
        var c2 = new Currency("EUR");

        c1.Should().NotBe(c2);
    }
}
