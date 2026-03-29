using Practice.Backend.CurrencyConverter.Domain.Exceptions;

namespace Practice.Backend.CurrencyConverter.Domain.Tests.Exceptions;

public sealed class InvalidCurrencyCodeExceptionSpecifications
{
    [Fact]
    public void Constructor_MessageProvided_StoresMessage()
    {
        var ex = new InvalidCurrencyCodeException("Currency code is invalid.", "currencyCode");

        ex.Message.Should().Contain("Currency code is invalid.");
    }

    [Fact]
    public void Constructor_ParamNameProvided_StoresParamName()
    {
        var ex = new InvalidCurrencyCodeException("msg", "currencyCode");

        ex.ParamName.Should().Be("currencyCode");
    }

    [Fact]
    public void Type_Always_InheritsFromDomainValidationException()
    {
        var ex = new InvalidCurrencyCodeException("msg", "param");

        ex.Should().BeAssignableTo<DomainValidationException>();
    }

    [Fact]
    public void Type_Always_InheritsFromArgumentException()
    {
        var ex = new InvalidCurrencyCodeException("msg", "param");

        ex.Should().BeAssignableTo<ArgumentException>();
    }
}
