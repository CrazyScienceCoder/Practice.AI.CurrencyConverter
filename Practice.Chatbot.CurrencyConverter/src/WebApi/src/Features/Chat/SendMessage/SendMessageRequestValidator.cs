using FluentValidation;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Features.Chat.SendMessage;

public sealed class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(4000).WithMessage("Message must not exceed 4000 characters.");

        RuleFor(x => x.ConversationId)
            .Must(id => id is null || Guid.TryParse(id, out _))
            .WithMessage("ConversationId must be a valid GUID if provided.");
    }
}
