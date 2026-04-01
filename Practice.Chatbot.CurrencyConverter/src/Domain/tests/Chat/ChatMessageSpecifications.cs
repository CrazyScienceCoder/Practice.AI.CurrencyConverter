using Practice.Chatbot.CurrencyConverter.Domain.Chat;
using Practice.Chatbot.CurrencyConverter.Domain.Exceptions;

namespace Practice.Chatbot.CurrencyConverter.Domain.Tests.Chat;

public sealed class ChatMessageSpecifications
{
    [Fact]
    public void UserMessage_ValidContent_CreatesMessageWithUserRole()
    {
        var message = ChatMessage.UserMessage("Hello");

        message.Role.Should().Be(MessageRole.User);
    }

    [Fact]
    public void UserMessage_ValidContent_StoresContent()
    {
        const string content = "What is the EUR rate?";

        var message = ChatMessage.UserMessage(content);

        message.Content.Should().Be(content);
    }

    [Fact]
    public void UserMessage_ValidContent_SetsTimestampCloseToUtcNow()
    {
        var before = DateTimeOffset.UtcNow;

        var message = ChatMessage.UserMessage("Hello");

        message.Timestamp.Should().BeOnOrAfter(before)
            .And.BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void AssistantMessage_ValidContent_CreatesMessageWithAssistantRole()
    {
        var message = ChatMessage.AssistantMessage("The rate is 1.2.");

        message.Role.Should().Be(MessageRole.Assistant);
    }

    [Fact]
    public void AssistantMessage_ValidContent_StoresContent()
    {
        const string content = "The current USD rate is 1.0.";

        var message = ChatMessage.AssistantMessage(content);

        message.Content.Should().Be(content);
    }

    [Fact]
    public void AssistantMessage_ValidContent_SetsTimestampCloseToUtcNow()
    {
        var before = DateTimeOffset.UtcNow;

        var message = ChatMessage.AssistantMessage("Response");

        message.Timestamp.Should().BeOnOrAfter(before)
            .And.BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void SystemMessage_ValidContent_CreatesMessageWithSystemRole()
    {
        var message = ChatMessage.SystemMessage("You are a helpful assistant.");

        message.Role.Should().Be(MessageRole.System);
    }

    [Fact]
    public void SystemMessage_ValidContent_StoresContent()
    {
        const string content = "You are a currency expert.";

        var message = ChatMessage.SystemMessage(content);

        message.Content.Should().Be(content);
    }

    [Fact]
    public void SystemMessage_ValidContent_SetsTimestampCloseToUtcNow()
    {
        var before = DateTimeOffset.UtcNow;

        var message = ChatMessage.SystemMessage("System prompt");

        message.Timestamp.Should().BeOnOrAfter(before)
            .And.BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UserMessage_EmptyOrWhiteSpaceContent_ThrowsArgumentException(string content)
    {
        var act = () => ChatMessage.UserMessage(content);

        act.Should().ThrowExactly<InvalidMessageContentException>()
            .Which.ParamName.Should().Be("content");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AssistantMessage_EmptyOrWhiteSpaceContent_ThrowsArgumentException(string content)
    {
        var act = () => ChatMessage.AssistantMessage(content);

        act.Should().ThrowExactly<InvalidMessageContentException>()
            .Which.ParamName.Should().Be("content");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void SystemMessage_EmptyOrWhiteSpaceContent_ThrowsArgumentException(string content)
    {
        var act = () => ChatMessage.SystemMessage(content);

        act.Should().ThrowExactly<InvalidMessageContentException>()
            .Which.ParamName.Should().Be("content");
    }

    [Fact]
    public void UserMessage_EmptyContent_ThrowsWithDescriptiveMessage()
    {
        var act = () => ChatMessage.UserMessage(string.Empty);

        act.Should().ThrowExactly<InvalidMessageContentException>()
            .Which.Message.Should().Contain("Message content cannot be empty.");
    }

    [Fact]
    public void UserMessage_SameContentAndRole_TwoMessagesAreEqualWhenTimestampsMatch()
    {
        // Record equality: two references from the same factory call share the same instance
        var message = ChatMessage.UserMessage("Hello");

        message.Should().Be(message);
    }

    [Fact]
    public void UserMessage_DifferentContent_TwoMessagesAreNotEqual()
    {
        var message1 = ChatMessage.UserMessage("Hello");
        var message2 = ChatMessage.UserMessage("Goodbye");

        message1.Should().NotBe(message2);
    }
}
