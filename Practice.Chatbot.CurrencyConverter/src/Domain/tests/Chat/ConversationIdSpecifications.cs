using Practice.Chatbot.CurrencyConverter.Domain.Chat;
using Practice.Chatbot.CurrencyConverter.Domain.Exceptions;

namespace Practice.Chatbot.CurrencyConverter.Domain.Tests.Chat;

public sealed class ConversationIdSpecifications
{
    [Fact]
    public void New_Always_ReturnsConversationIdWithNonEmptyGuid()
    {
        var id = ConversationId.New();

        id.Value.Should().NotBeEmpty();
    }

    [Fact]
    public void New_CalledTwice_ReturnsDifferentIds()
    {
        var id1 = ConversationId.New();
        var id2 = ConversationId.New();

        id1.Should().NotBe(id2);
    }

    [Fact]
    public void From_ValidGuidString_ReturnsConversationIdWithMatchingValue()
    {
        var guid = Guid.NewGuid();
        var guidString = guid.ToString();

        var id = ConversationId.From(guidString);

        id.Value.Should().Be(guid);
    }

    [Theory]
    [InlineData("not-a-guid")]
    [InlineData("12345")]
    [InlineData("")]
    public void From_InvalidGuidString_ThrowsArgumentException(string invalidValue)
    {
        var act = () => ConversationId.From(invalidValue);

        act.Should().ThrowExactly<InvalidConversationIdException>()
            .Which.ParamName.Should().Be("value");
    }

    [Fact]
    public void From_InvalidGuidString_ThrowsWithDescriptiveMessage()
    {
        const string invalidValue = "not-a-guid";

        var act = () => ConversationId.From(invalidValue);

        act.Should().ThrowExactly<InvalidConversationIdException>()
            .Which.Message.Should().Contain("is not a valid ConversationId");
    }

    [Fact]
    public void ToString_Always_ReturnsGuidAsString()
    {
        var guid = Guid.NewGuid();
        var id = new ConversationId(guid);

        var result = id.ToString();

        result.Should().Be(guid.ToString());
    }

    [Fact]
    public void Equals_SameGuidValue_ReturnsTrue()
    {
        var guid = Guid.NewGuid();
        var id1 = new ConversationId(guid);
        var id2 = new ConversationId(guid);

        id1.Should().Be(id2);
    }

    [Fact]
    public void Equals_DifferentGuidValues_ReturnsFalse()
    {
        var id1 = ConversationId.New();
        var id2 = ConversationId.New();

        id1.Should().NotBe(id2);
    }

    [Fact]
    public void ImplicitOperator_FromGuid_CreatesConversationId()
    {
        var guid = Guid.NewGuid();

        ConversationId id = guid;

        id.Value.Should().Be(guid);
    }

    [Fact]
    public void ImplicitOperator_ToGuid_ReturnsUnderlyingValue()
    {
        var guid = Guid.NewGuid();
        var id = new ConversationId(guid);

        Guid result = id;

        result.Should().Be(guid);
    }
}
