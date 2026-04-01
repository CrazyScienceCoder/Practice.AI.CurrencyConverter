using Practice.Chatbot.CurrencyConverter.Domain.Exceptions;

namespace Practice.Chatbot.CurrencyConverter.Domain.Tests.Exceptions;

public sealed class DomainValidationExceptionSpecifications
{
    [Fact]
    public void Type_Always_InheritsFromArgumentException()
    {
        DomainValidationException ex = new InvalidUserIdException("msg", "param");

        ex.Should().BeAssignableTo<ArgumentException>();
    }
}
