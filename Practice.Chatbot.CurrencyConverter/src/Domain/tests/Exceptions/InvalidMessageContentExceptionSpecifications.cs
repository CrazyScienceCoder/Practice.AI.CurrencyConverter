using Practice.Chatbot.CurrencyConverter.Domain.Exceptions;

namespace Practice.Chatbot.CurrencyConverter.Domain.Tests.Exceptions;

public sealed class InvalidMessageContentExceptionSpecifications
{
    [Fact]
    public void Constructor_MessageProvided_StoresMessage()
    {
        var ex = new InvalidMessageContentException("Message content cannot be empty.", "content");

        ex.Message.Should().Contain("Message content cannot be empty.");
    }

    [Fact]
    public void Constructor_ParamNameProvided_StoresParamName()
    {
        var ex = new InvalidMessageContentException("msg", "content");

        ex.ParamName.Should().Be("content");
    }

    [Fact]
    public void Type_Always_InheritsFromDomainValidationException()
    {
        var ex = new InvalidMessageContentException("msg", "param");

        ex.Should().BeAssignableTo<DomainValidationException>();
    }

    [Fact]
    public void Type_Always_InheritsFromArgumentException()
    {
        var ex = new InvalidMessageContentException("msg", "param");

        ex.Should().BeAssignableTo<ArgumentException>();
    }
}
