using MediatR;
using Microsoft.Extensions.Logging;
using Practice.Chatbot.CurrencyConverter.Application.Abstractions;
using Practice.Chatbot.CurrencyConverter.Domain.Chat;
using Practice.Chatbot.CurrencyConverter.Domain.Contracts;

namespace Practice.Chatbot.CurrencyConverter.Application.Chat.Send;

public sealed class SendChatMessageCommandHandler(
    IChatHistoryRepository repository,
    IChatOrchestrator orchestrator,
    ILogger<SendChatMessageCommandHandler> logger)
    : IStreamRequestHandler<SendChatMessageCommand, string>
{
    public async IAsyncEnumerable<string> Handle(
        SendChatMessageCommand command,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["UserId"] = command.UserId,
            ["ConversationId"] = command.ConversationId ?? "new"
        });

        Conversation conversation;
        if (command.ConversationId is not null)
        {
            var id = ConversationId.From(command.ConversationId);
            var existing = await repository.FindAsync(id, cancellationToken);
            if (existing is not null)
            {
                conversation = existing;
                logger.LogInformation("Loaded existing conversation {ConversationId}", id);
            }
            else
            {
                conversation = Conversation.StartWithId(id, command.UserId);
                logger.LogInformation("Started new conversation {ConversationId}", id);
            }
        }
        else
        {
            conversation = Conversation.Start(command.UserId);
            logger.LogInformation("Started new conversation {ConversationId}", conversation.Id);
        }

        conversation.AddUserMessage(command.UserMessage);

        var fullReply = new System.Text.StringBuilder();
        await foreach (var chunk in orchestrator.StreamReplyAsync(conversation, cancellationToken))
        {
            fullReply.Append(chunk);
            yield return chunk;
        }

        conversation.AddAssistantMessage(fullReply.ToString());
        await repository.SaveAsync(conversation, cancellationToken);

        logger.LogInformation("Conversation {ConversationId} saved with {MessageCount} messages",
            conversation.Id, conversation.Messages.Count);
    }
}
