using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Practice.Chatbot.CurrencyConverter.Domain.Chat;
using Practice.Chatbot.CurrencyConverter.Domain.Contracts;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Chat;

public sealed class RedisChatHistoryRepository(
    IDistributedCache cache,
    ILogger<RedisChatHistoryRepository> logger)
    : IChatHistoryRepository
{
    private static readonly TimeSpan Ttl = TimeSpan.FromHours(24);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    private static string CacheKey(ConversationId id) => $"chat:{id}";

    public async Task<Conversation?> FindAsync(ConversationId id, CancellationToken cancellationToken = default)
    {
        var json = await cache.GetStringAsync(CacheKey(id), cancellationToken);
        if (json is null)
        {
            logger.LogDebug("Conversation {ConversationId} not found in cache", id);
            return null;
        }

        var dto = JsonSerializer.Deserialize<ConversationDto>(json, JsonOptions);
        if (dto is null)
        {
            return null;
        }

        var messages = dto.Messages.Select(m => m.Role switch
        {
            MessageRole.User => ChatMessage.UserMessage(m.Content),
            MessageRole.Assistant => ChatMessage.AssistantMessage(m.Content),
            _ => ChatMessage.SystemMessage(m.Content)
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

        var json = JsonSerializer.Serialize(dto, JsonOptions);

        await cache.SetStringAsync(
            CacheKey(conversation.Id),
            json,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = Ttl },
            cancellationToken);

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
