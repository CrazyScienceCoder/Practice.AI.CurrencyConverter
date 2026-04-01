using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Practice.Chatbot.CurrencyConverter.Integration.Tests.Infrastructure;

namespace Practice.Chatbot.CurrencyConverter.Integration.Tests.Chat.GetHistory;

public partial class GetHistoryIntegrationSpecifications
{
    [Fact]
    public async Task GetHistoryAsync_WithoutAuthorizationHeader_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var conversationId = Guid.NewGuid().ToString();

        var response = await _client.GetAsync(
            HistoryUrl(conversationId),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHistoryAsync_WithInvalidBearerToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "this-is-not-a-valid-token");
        var conversationId = Guid.NewGuid().ToString();

        var response = await _client.GetAsync(
            HistoryUrl(conversationId),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHistoryAsync_WithTokenMissingRequiredRole_Returns403()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken("wrong:role"));
        var conversationId = Guid.NewGuid().ToString();

        var response = await _client.GetAsync(
            HistoryUrl(conversationId),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetHistoryAsync_WithNonExistingConversationId_Returns404NotFound()
    {
        AuthorizeClient();
        var conversationId = Guid.NewGuid().ToString();

        var response = await _client.GetAsync(
            HistoryUrl(conversationId),
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonNode.Parse(body)!;

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetHistoryAsync_WithExistingConversation_Returns200WithMessages()
    {
        AuthorizeClient();
        var conversationId = Guid.NewGuid().ToString();

        await SeedConversationAsync(conversationId, JwtTokenHelper.TestUserId, [
            ("User", "What is the EUR rate?"),
            ("Assistant", "The EUR rate is 0.92.")
        ]);

        var response = await _client.GetAsync(
            HistoryUrl(conversationId),
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonNode.Parse(body)!;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        json["messages"]!.AsArray().Should().HaveCount(2);
    }

    [Fact]
    public async Task GetHistoryAsync_WithExistingConversation_ReturnsCorrectMessageContent()
    {
        AuthorizeClient();
        var conversationId = Guid.NewGuid().ToString();

        await SeedConversationAsync(conversationId, JwtTokenHelper.TestUserId, [
            ("User", "What is the GBP rate?"),
            ("Assistant", "The GBP rate is 0.79.")
        ]);

        var response = await _client.GetAsync(
            HistoryUrl(conversationId),
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonNode.Parse(body)!;

        var messages = json["messages"]!.AsArray();
        messages[0]!["role"]!.GetValue<string>().Should().Be("user");
        messages[0]!["content"]!.GetValue<string>().Should().Be("What is the GBP rate?");
        messages[1]!["role"]!.GetValue<string>().Should().Be("assistant");
        messages[1]!["content"]!.GetValue<string>().Should().Be("The GBP rate is 0.79.");
    }

    [Fact]
    public async Task GetHistoryAsync_WithExistingConversation_ReturnsCorrectConversationId()
    {
        AuthorizeClient();
        var conversationId = Guid.NewGuid().ToString();

        await SeedConversationAsync(conversationId, JwtTokenHelper.TestUserId, [
            ("User", "Hello")
        ]);

        var response = await _client.GetAsync(
            HistoryUrl(conversationId),
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonNode.Parse(body)!;

        json["conversationId"]!.GetValue<string>().Should().Be(conversationId);
    }

    [Fact]
    public async Task GetHistoryAsync_WithConversationOwnedByDifferentUser_Returns404NotFound()
    {
        AuthorizeClient();
        var conversationId = Guid.NewGuid().ToString();

        await SeedConversationAsync(conversationId, "another-user-id", [
            ("User", "Private message")
        ]);

        var response = await _client.GetAsync(
            HistoryUrl(conversationId),
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonNode.Parse(body)!;

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
