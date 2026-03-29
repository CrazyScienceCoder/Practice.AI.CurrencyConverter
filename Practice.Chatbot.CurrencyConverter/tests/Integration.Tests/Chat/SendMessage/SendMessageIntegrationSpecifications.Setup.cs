using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Practice.Chatbot.CurrencyConverter.Domain.Chat;
using Practice.Chatbot.CurrencyConverter.Integration.Tests.Infrastructure;

namespace Practice.Chatbot.CurrencyConverter.Integration.Tests.Chat.SendMessage;

public partial class SendMessageIntegrationSpecifications(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private const string SendMessageUrl = "/api/v1/chat/message";
    private const string AiChatRole = "ai:chat";

    private readonly HttpClient _client = factory.CreateClient();

    private void AuthorizeClient()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(AiChatRole));
    }

    private static StringContent BuildRequestBody(string message, string? conversationId = null)
    {
        var payload = new { message, conversationId };
        return new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");
    }

    private void SetupOrchestratorReply(params string[] chunks)
    {
        factory.ChatOrchestratorMock
            .Setup(o => o.StreamReplyAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(chunks.Length > 0 ? chunks : ["Hello from the AI."]));
    }

    private static async IAsyncEnumerable<string> ToAsyncEnumerable(string[] chunks)
    {
        foreach (var chunk in chunks)
            yield return chunk;

        await Task.CompletedTask;
    }
}
