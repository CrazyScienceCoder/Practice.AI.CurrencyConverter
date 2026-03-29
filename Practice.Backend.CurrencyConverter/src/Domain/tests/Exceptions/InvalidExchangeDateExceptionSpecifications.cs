using Practice.Backend.CurrencyConverter.Domain.Exceptions;

namespace Practice.Backend.CurrencyConverter.Domain.Tests.Exceptions;

public sealed class InvalidExchangeDateExceptionSpecifications
{
    [Fact]
    public void Constructor_MessageProvided_StoresMessage()
    {
        var ex = new InvalidExchangeDateException("Date is invalid.", "dateUtc");

        ex.Message.Should().Contain("Date is invalid.");
    }

    [Fact]
    public void Constructor_ParamNameProvided_StoresParamName()
    {
        var ex = new InvalidExchangeDateException("msg", "dateUtc");

        ex.ParamName.Should().Be("dateUtc");
    }

    [Fact]
    public void Type_Always_InheritsFromDomainValidationException()
    {
        var ex = new InvalidExchangeDateException("msg", "param");

        ex.Should().BeAssignableTo<DomainValidationException>();
    }

    [Fact]
    public void Type_Always_InheritsFromArgumentException()
    {
        var ex = new InvalidExchangeDateException("msg", "param");

        ex.Should().BeAssignableTo<ArgumentException>();
    }
}
