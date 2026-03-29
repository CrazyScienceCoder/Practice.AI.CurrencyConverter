using Practice.Backend.CurrencyConverter.Domain.Exceptions;

namespace Practice.Backend.CurrencyConverter.Domain.Tests.Exceptions;

public sealed class InvalidAmountExceptionSpecifications
{
    [Fact]
    public void Constructor_MessageProvided_StoresMessage()
    {
        var ex = new InvalidAmountException("Amount cannot be negative.", "amount");

        ex.Message.Should().Contain("Amount cannot be negative.");
    }

    [Fact]
    public void Constructor_ParamNameProvided_StoresParamName()
    {
        var ex = new InvalidAmountException("msg", "amount");

        ex.ParamName.Should().Be("amount");
    }

    [Fact]
    public void Type_Always_InheritsFromDomainValidationException()
    {
        var ex = new InvalidAmountException("msg", "param");

        ex.Should().BeAssignableTo<DomainValidationException>();
    }

    [Fact]
    public void Type_Always_InheritsFromArgumentException()
    {
        var ex = new InvalidAmountException("msg", "param");

        ex.Should().BeAssignableTo<ArgumentException>();
    }
}
