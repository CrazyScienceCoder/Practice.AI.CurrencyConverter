using Practice.Chatbot.CurrencyConverter.Domain.Exceptions;

namespace Practice.Chatbot.CurrencyConverter.Domain.Tests.Exceptions;

public sealed class InvalidConversationIdExceptionSpecifications
{
    [Fact]
    public void Constructor_MessageProvided_StoresMessage()
    {
        var ex = new InvalidConversationIdException("'abc' is not a valid ConversationId.", "value");

        ex.Message.Should().Contain("'abc' is not a valid ConversationId.");
    }

    [Fact]
    public void Constructor_ParamNameProvided_StoresParamName()
    {
        var ex = new InvalidConversationIdException("msg", "value");

        ex.ParamName.Should().Be("value");
    }

    [Fact]
    public void Type_Always_InheritsFromDomainValidationException()
    {
        var ex = new InvalidConversationIdException("msg", "param");

        ex.Should().BeAssignableTo<DomainValidationException>();
    }

    [Fact]
    public void Type_Always_InheritsFromArgumentException()
    {
        var ex = new InvalidConversationIdException("msg", "param");

        ex.Should().BeAssignableTo<ArgumentException>();
    }
}
