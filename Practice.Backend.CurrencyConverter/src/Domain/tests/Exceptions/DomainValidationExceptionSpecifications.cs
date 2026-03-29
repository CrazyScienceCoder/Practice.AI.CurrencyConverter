using Practice.Backend.CurrencyConverter.Domain.Exceptions;

namespace Practice.Backend.CurrencyConverter.Domain.Tests.Exceptions;

public sealed class DomainValidationExceptionSpecifications
{
    [Fact]
    public void Type_Always_InheritsFromArgumentException()
    {
        DomainValidationException ex = new InvalidCurrencyCodeException("msg", "param");

        ex.Should().BeAssignableTo<ArgumentException>();
    }
}
