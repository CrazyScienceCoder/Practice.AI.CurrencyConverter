using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Practice.Chatbot.CurrencyConverter.Domain.Chat;
using Practice.Chatbot.CurrencyConverter.Infrastructure.Chat;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Tests.Chat;

public sealed partial class RedisChatHistoryRepositorySpecifications
{
    // Mirrors the private ConversationDto / MessageDto inside RedisChatHistoryRepository
    private sealed record TestConversationDto(
        string Id,
        string UserId,
        DateTimeOffset CreatedAt,
        List<TestMessageDto> Messages);

    private sealed record TestMessageDto(MessageRole Role, string Content, DateTimeOffset Timestamp);

    private static readonly JsonSerializerOptions TestJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    private class TestBuilder
    {
        public Mock<IDistributedCache> CacheMock { get; } = new();
        private readonly Mock<ILogger<RedisChatHistoryRepository>> _loggerMock = new();

        public TestBuilder()
        {
            CacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);

            CacheMock
                .Setup(c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public TestBuilder WithCacheMiss()
        {
            CacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);
            return this;
        }

        public TestBuilder WithCachedConversation(
            Guid id,
            string userId,
            DateTimeOffset createdAt,
            IEnumerable<TestMessageDto> messages)
        {
            var dto = new TestConversationDto(
                id.ToString(),
                userId,
                createdAt,
                messages.ToList());

            var json = JsonSerializer.Serialize(dto, TestJsonOptions);
            var bytes = Encoding.UTF8.GetBytes(json);

            CacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(bytes);
            return this;
        }

        public RedisChatHistoryRepository Build() => new(CacheMock.Object, _loggerMock.Object);
    }
}
