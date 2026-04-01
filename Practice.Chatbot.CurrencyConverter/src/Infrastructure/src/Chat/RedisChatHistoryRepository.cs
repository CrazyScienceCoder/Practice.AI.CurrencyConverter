using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Practice.Chatbot.CurrencyConverter.Domain.Chat;
using Practice.Chatbot.CurrencyConverter.Domain.Contracts;
using Practice.Chatbot.CurrencyConverter.Infrastructure.Configurations;
using Practice.Chatbot.CurrencyConverter.Infrastructure.Extensions;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Chat;

public sealed class RedisChatHistoryRepository(
    IDistributedCache cache,
    IOptions<ChatConfiguration> chatOptions,
    ILogger<RedisChatHistoryRepository> logger)
    : IChatHistoryRepository
{
    private static string CacheKey(ConversationId id) => $"chat:{id}";

    public async Task<Conversation?> FindAsync(ConversationId id, CancellationToken cancellationToken = default)
    {
        var dto = await cache.GetAsync<ConversationDto>(CacheKey(id), logger, cancellationToken);
        if (dto is null)
        {
            logger.LogDebug("Conversation {ConversationId} not found in cache", id);
            return null;
        }

        var messages = dto.Messages.Select(m =>
        {
            if (m.Role == MessageRole.User) return ChatMessage.UserMessage(m.Content);
            if (m.Role == MessageRole.Assistant) return ChatMessage.AssistantMessage(m.Content);
            return ChatMessage.SystemMessage(m.Content);
        });

        return Conversation.Reconstitute(
            ConversationId.From(dto.Id),
            dto.UserId,
            dto.CreatedAt,
            messages);
    }

    public async Task SaveAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        var dto = new ConversationDto(
            conversation.Id.ToString(),
            conversation.UserId,
            conversation.CreatedAt,
            conversation.Messages.Select(m => new MessageDto(m.Role, m.Content, m.Timestamp)).ToList()
        );

        var entryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = chatOptions.Value.ConversationTtl
        };

        await cache.SetAsync(CacheKey(conversation.Id), dto, entryOptions, logger, cancellationToken);

        logger.LogDebug("Saved conversation {ConversationId} with {Count} messages",
            conversation.Id, conversation.Messages.Count);
    }

    private sealed record ConversationDto(
        string Id,
        string UserId,
        DateTimeOffset CreatedAt,
        List<MessageDto> Messages);

    private sealed record MessageDto(MessageRole Role, string Content, DateTimeOffset Timestamp);
}
