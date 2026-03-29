using Practice.Chatbot.CurrencyConverter.Domain.Chat;

namespace Practice.Chatbot.CurrencyConverter.Domain.Tests.Chat;

public sealed class MessageRoleSpecifications
{
    [Fact]
    public void System_HasValueZero()
    {
        ((int)MessageRole.System).Should().Be(0);
    }

    [Fact]
    public void User_HasValueOne()
    {
        ((int)MessageRole.User).Should().Be(1);
    }

    [Fact]
    public void Assistant_HasValueTwo()
    {
        ((int)MessageRole.Assistant).Should().Be(2);
    }

    [Fact]
    public void MessageRole_Always_HasExactlyThreeValues()
    {
        var values = Enum.GetValues<MessageRole>();

        values.Should().HaveCount(3);
    }
}
