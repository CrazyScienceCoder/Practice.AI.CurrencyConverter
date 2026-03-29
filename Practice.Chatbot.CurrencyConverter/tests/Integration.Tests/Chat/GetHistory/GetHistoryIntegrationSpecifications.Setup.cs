using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Practice.Chatbot.CurrencyConverter.Integration.Tests.Infrastructure;

namespace Practice.Chatbot.CurrencyConverter.Integration.Tests.Chat.GetHistory;

public partial class GetHistoryIntegrationSpecifications(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private const string AiChatRole = "ai:chat";

    private readonly HttpClient _client = factory.CreateClient();

    private static string HistoryUrl(string conversationId)
        => $"/api/v1/chat/{conversationId}/history";

    private void AuthorizeClient()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(AiChatRole));
    }

    private async Task SeedConversationAsync(
        string conversationId,
        string userId,
        IReadOnlyList<(string Role, string Content)> messages)
    {
        var timestamp = new DateTimeOffset(2025, 1, 15, 10, 0, 0, TimeSpan.Zero);

        var msgDtos = messages.Select(m => new
        {
            role = m.Role,
            content = m.Content,
            timestamp
        }).ToList();

        var dto = new
        {
            id = conversationId,
            userId,
            createdAt = timestamp,
            messages = msgDtos
        };

        var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var cache = factory.Services.GetRequiredService<IDistributedCache>();
        await cache.SetStringAsync(
            $"chat:{conversationId}",
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            },
            TestContext.Current.CancellationToken);
    }
}
