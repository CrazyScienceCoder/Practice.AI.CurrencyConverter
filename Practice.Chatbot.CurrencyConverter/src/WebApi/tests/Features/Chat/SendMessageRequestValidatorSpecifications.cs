using Practice.Chatbot.CurrencyConverter.WebApi.Features.Chat.SendMessage;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Tests.Features.Chat;

public class SendMessageRequestValidatorSpecifications
{
    private readonly SendMessageRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequestWithGuidConversationId_Passes()
    {
        var request = new SendMessageRequest(
            ConversationId: "550e8400-e29b-41d4-a716-446655440000",
            Message: "Hello, what is the exchange rate?");

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ValidRequestWithNullConversationId_Passes()
    {
        var request = new SendMessageRequest(
            ConversationId: null,
            Message: "Hello, what is the exchange rate?");

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyMessage_Fails()
    {
        var request = new SendMessageRequest(
            ConversationId: null,
            Message: "");

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EmptyMessage_HasCorrectErrorMessage()
    {
        var request = new SendMessageRequest(
            ConversationId: null,
            Message: "");

        var result = _validator.Validate(request);

        result.Errors.Should().ContainSingle(e =>
            e.PropertyName == nameof(SendMessageRequest.Message) &&
            e.ErrorMessage == "Message is required.");
    }

    [Fact]
    public void Validate_MessageExceedsMaxLength_Fails()
    {
        var request = new SendMessageRequest(
            ConversationId: null,
            Message: new string('x', 4001));

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_MessageExceedsMaxLength_HasCorrectErrorMessage()
    {
        var request = new SendMessageRequest(
            ConversationId: null,
            Message: new string('x', 4001));

        var result = _validator.Validate(request);

        result.Errors.Should().ContainSingle(e =>
            e.PropertyName == nameof(SendMessageRequest.Message) &&
            e.ErrorMessage == "Message must not exceed 4000 characters.");
    }

    [Fact]
    public void Validate_MessageAtMaxLength_Passes()
    {
        var request = new SendMessageRequest(
            ConversationId: null,
            Message: new string('x', 4000));

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidConversationId_Fails()
    {
        var request = new SendMessageRequest(
            ConversationId: "not-a-valid-guid",
            Message: "Hello");

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_InvalidConversationId_HasCorrectErrorMessage()
    {
        var request = new SendMessageRequest(
            ConversationId: "not-a-valid-guid",
            Message: "Hello");

        var result = _validator.Validate(request);

        result.Errors.Should().ContainSingle(e =>
            e.PropertyName == nameof(SendMessageRequest.ConversationId) &&
            e.ErrorMessage == "ConversationId must be a valid GUID if provided.");
    }

    [Fact]
    public void Validate_EmptyStringConversationId_Fails()
    {
        var request = new SendMessageRequest(
            ConversationId: "",
            Message: "Hello");

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ValidGuidUpperCaseConversationId_Passes()
    {
        var request = new SendMessageRequest(
            ConversationId: "550E8400-E29B-41D4-A716-446655440000",
            Message: "Hello");

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }
}
