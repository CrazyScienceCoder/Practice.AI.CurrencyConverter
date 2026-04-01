using Practice.Chatbot.CurrencyConverter.Domain.Exceptions;

namespace Practice.Chatbot.CurrencyConverter.Domain.Tests.Exceptions;

public sealed class InvalidUserIdExceptionSpecifications
{
    [Fact]
    public void Constructor_MessageProvided_StoresMessage()
    {
        var ex = new InvalidUserIdException("UserId cannot be empty.", "userId");

        ex.Message.Should().Contain("UserId cannot be empty.");
    }

    [Fact]
    public void Constructor_ParamNameProvided_StoresParamName()
    {
        var ex = new InvalidUserIdException("msg", "userId");

        ex.ParamName.Should().Be("userId");
    }

    [Fact]
    public void Type_Always_InheritsFromDomainValidationException()
    {
        var ex = new InvalidUserIdException("msg", "param");

        ex.Should().BeAssignableTo<DomainValidationException>();
    }

    [Fact]
    public void Type_Always_InheritsFromArgumentException()
    {
        var ex = new InvalidUserIdException("msg", "param");

        ex.Should().BeAssignableTo<ArgumentException>();
    }
}
