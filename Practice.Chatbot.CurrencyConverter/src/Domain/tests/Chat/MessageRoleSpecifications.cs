using Practice.Chatbot.CurrencyConverter.Domain.Chat;

namespace Practice.Chatbot.CurrencyConverter.Domain.Tests.Chat;

public sealed class MessageRoleSpecifications
{
    [Fact]
    public void System_HasValueZero()
    {
        MessageRole.System.Value.Should().Be(0);
    }

    [Fact]
    public void User_HasValueOne()
    {
        MessageRole.User.Value.Should().Be(1);
    }

    [Fact]
    public void Assistant_HasValueTwo()
    {
        MessageRole.Assistant.Value.Should().Be(2);
    }

    [Fact]
    public void MessageRole_Always_HasExactlyThreeValues()
    {
        MessageRole.List.Should().HaveCount(3);
    }

    [Fact]
    public void System_HasExpectedName()
    {
        MessageRole.System.Name.Should().Be(nameof(MessageRole.System));
    }

    [Fact]
    public void User_HasExpectedName()
    {
        MessageRole.User.Name.Should().Be(nameof(MessageRole.User));
    }

    [Fact]
    public void Assistant_HasExpectedName()
    {
        MessageRole.Assistant.Name.Should().Be(nameof(MessageRole.Assistant));
    }
}
