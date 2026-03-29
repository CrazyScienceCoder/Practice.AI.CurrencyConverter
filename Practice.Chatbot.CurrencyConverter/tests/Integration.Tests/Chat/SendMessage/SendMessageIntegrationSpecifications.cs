using System.Net;
using System.Net.Http.Headers;
using Practice.Chatbot.CurrencyConverter.Integration.Tests.Infrastructure;

namespace Practice.Chatbot.CurrencyConverter.Integration.Tests.Chat.SendMessage;

public partial class SendMessageIntegrationSpecifications
{
    [Fact]
    public async Task SendMessageAsync_WithoutAuthorizationHeader_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody("What is the USD exchange rate?"),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SendMessageAsync_WithInvalidBearerToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "this-is-not-a-valid-token");

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody("What is the USD exchange rate?"),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SendMessageAsync_WithTokenMissingRequiredRole_Returns403()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken("wrong:role"));

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody("What is the USD exchange rate?"),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SendMessageAsync_WithEmptyMessage_Returns400()
    {
        AuthorizeClient();

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody(""),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SendMessageAsync_WithMessageExceedingMaxLength_Returns400()
    {
        AuthorizeClient();

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody(new string('x', 4001)),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SendMessageAsync_WithInvalidConversationIdGuid_Returns400()
    {
        AuthorizeClient();

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody("Hello", "not-a-valid-guid"),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SendMessageAsync_WithValidNewConversation_Returns200()
    {
        SetupOrchestratorReply("The USD rate is 1.0.");
        AuthorizeClient();

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody("What is the USD exchange rate?"),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendMessageAsync_WithValidNewConversation_ReturnsSseContentTypeHeader()
    {
        SetupOrchestratorReply("The rate is 1.0.");
        AuthorizeClient();

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody("What is the rate?"),
            TestContext.Current.CancellationToken);

        response.Content.Headers.ContentType!.MediaType.Should().Be("text/event-stream");
    }

    [Fact]
    public async Task SendMessageAsync_WithValidNewConversation_ReturnsCacheControlHeader()
    {
        SetupOrchestratorReply("The rate is 1.0.");
        AuthorizeClient();

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody("What is the rate?"),
            TestContext.Current.CancellationToken);

        response.Headers.CacheControl!.NoCache.Should().BeTrue();
    }

    [Fact]
    public async Task SendMessageAsync_WithValidNewConversation_ReturnsXAccelBufferingHeader()
    {
        SetupOrchestratorReply("The rate is 1.0.");
        AuthorizeClient();

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody("What is the rate?"),
            TestContext.Current.CancellationToken);

        response.Headers.TryGetValues("X-Accel-Buffering", out var values).Should().BeTrue();
        values!.Should().ContainSingle(v => v == "no");
    }

    [Fact]
    public async Task SendMessageAsync_WithValidNewConversation_ResponseBodyContainsSseChunks()
    {
        SetupOrchestratorReply("Hello", " World");
        AuthorizeClient();

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody("Hi"),
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        body.Should().Contain("data: Hello");
        body.Should().Contain("data:  World");
    }

    [Fact]
    public async Task SendMessageAsync_WithValidNewConversation_ResponseBodyEndsWithDoneSignal()
    {
        SetupOrchestratorReply("Some response.");
        AuthorizeClient();

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody("Hello"),
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        body.Should().Contain("data: [DONE]");
    }

    [Fact]
    public async Task SendMessageAsync_WithChunkContainingNewline_EscapesNewlineInSseBody()
    {
        SetupOrchestratorReply("Line1\nLine2");
        AuthorizeClient();

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody("Hello"),
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        body.Should().Contain(@"data: Line1\nLine2");
    }

    [Fact]
    public async Task SendMessageAsync_WithExistingConversationId_Returns200()
    {
        SetupOrchestratorReply("Following up on that.");
        AuthorizeClient();

        var existingId = Guid.NewGuid().ToString();

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody("Tell me more", existingId),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendMessageAsync_WithNullConversationId_CreatesNewConversation()
    {
        SetupOrchestratorReply("Starting fresh.");
        AuthorizeClient();

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody("Hello", null),
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("data: [DONE]");
    }

    [Fact]
    public async Task SendMessageAsync_WithMessageAtMaxLength_Returns200()
    {
        SetupOrchestratorReply("OK.");
        AuthorizeClient();

        var response = await _client.PostAsync(
            SendMessageUrl,
            BuildRequestBody(new string('x', 4000)),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
