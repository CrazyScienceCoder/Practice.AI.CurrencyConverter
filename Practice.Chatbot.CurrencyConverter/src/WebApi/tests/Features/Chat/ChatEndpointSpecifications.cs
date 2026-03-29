using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice.Chatbot.CurrencyConverter.Application.Chat.GetHistory;
using Practice.Chatbot.CurrencyConverter.Application.Chat.Send;
using Practice.Chatbot.CurrencyConverter.WebApi.Features.Chat.SendMessage;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Tests.Features.Chat;

public partial class ChatEndpointSpecifications
{
    [Fact]
    public async Task SendMessageAsync_WithNameIdentifierClaim_UsesClaimAsUserId()
    {
        var testBuilder = new TestBuilder()
            .SetupStreamingResponse([]);
        var (endpoint, _) = testBuilder.Build(userId: "user-abc", claimType: ClaimTypes.NameIdentifier);

        await endpoint.SendMessageAsync(testBuilder.DefaultSendRequest, TestContext.Current.CancellationToken);

        testBuilder.MediatorMock.Verify(
            m => m.CreateStream(
                It.Is<SendChatMessageCommand>(c => c.UserId == "user-abc"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_WithSubClaim_UsesSubClaimAsUserId()
    {
        var testBuilder = new TestBuilder()
            .SetupStreamingResponse([]);
        var (endpoint, _) = testBuilder.Build(userId: "sub-user", claimType: "sub");

        await endpoint.SendMessageAsync(testBuilder.DefaultSendRequest, TestContext.Current.CancellationToken);

        testBuilder.MediatorMock.Verify(
            m => m.CreateStream(
                It.Is<SendChatMessageCommand>(c => c.UserId == "sub-user"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_WithNoClaims_UsesAnonymousAsUserId()
    {
        var testBuilder = new TestBuilder()
            .SetupStreamingResponse([]);
        var (endpoint, _) = testBuilder.Build(userId: null);

        await endpoint.SendMessageAsync(testBuilder.DefaultSendRequest, TestContext.Current.CancellationToken);

        testBuilder.MediatorMock.Verify(
            m => m.CreateStream(
                It.Is<SendChatMessageCommand>(c => c.UserId == "anonymous"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_SetsContentTypeHeaderToEventStream()
    {
        var testBuilder = new TestBuilder()
            .SetupStreamingResponse([]);
        var (endpoint, httpContext) = testBuilder.Build();

        await endpoint.SendMessageAsync(testBuilder.DefaultSendRequest, TestContext.Current.CancellationToken);

        httpContext.Response.Headers["Content-Type"].ToString().Should().Be("text/event-stream");
    }

    [Fact]
    public async Task SendMessageAsync_SetsCacheControlHeaderToNoCache()
    {
        var testBuilder = new TestBuilder()
            .SetupStreamingResponse([]);
        var (endpoint, httpContext) = testBuilder.Build();

        await endpoint.SendMessageAsync(testBuilder.DefaultSendRequest, TestContext.Current.CancellationToken);

        httpContext.Response.Headers["Cache-Control"].ToString().Should().Be("no-cache");
    }

    [Fact]
    public async Task SendMessageAsync_SetsXAccelBufferingHeaderToNo()
    {
        var testBuilder = new TestBuilder()
            .SetupStreamingResponse([]);
        var (endpoint, httpContext) = testBuilder.Build();

        await endpoint.SendMessageAsync(testBuilder.DefaultSendRequest, TestContext.Current.CancellationToken);

        httpContext.Response.Headers["X-Accel-Buffering"].ToString().Should().Be("no");
    }

    [Fact]
    public async Task SendMessageAsync_WritesChunksInSseFormat()
    {
        var testBuilder = new TestBuilder()
            .SetupStreamingResponse(["Hello", "World"]);
        var (endpoint, httpContext) = testBuilder.Build();

        await endpoint.SendMessageAsync(testBuilder.DefaultSendRequest, TestContext.Current.CancellationToken);

        var body = await ReadResponseBodyAsync(httpContext);
        body.Should().Contain("data: Hello\n\n");
        body.Should().Contain("data: World\n\n");
    }

    [Fact]
    public async Task SendMessageAsync_EscapesNewlinesInChunks()
    {
        var testBuilder = new TestBuilder()
            .SetupStreamingResponse(["line1\nline2"]);
        var (endpoint, httpContext) = testBuilder.Build();

        await endpoint.SendMessageAsync(testBuilder.DefaultSendRequest, TestContext.Current.CancellationToken);

        var body = await ReadResponseBodyAsync(httpContext);
        body.Should().Contain("data: line1\\nline2\n\n");
    }

    [Fact]
    public async Task SendMessageAsync_EscapesCarriageReturnsInChunks()
    {
        var testBuilder = new TestBuilder()
            .SetupStreamingResponse(["line1\rline2"]);
        var (endpoint, httpContext) = testBuilder.Build();

        await endpoint.SendMessageAsync(testBuilder.DefaultSendRequest, TestContext.Current.CancellationToken);

        var body = await ReadResponseBodyAsync(httpContext);
        body.Should().Contain("data: line1\\rline2\n\n");
    }

    [Fact]
    public async Task SendMessageAsync_WritesDoneMessageAtEnd()
    {
        var testBuilder = new TestBuilder()
            .SetupStreamingResponse(["chunk"]);
        var (endpoint, httpContext) = testBuilder.Build();

        await endpoint.SendMessageAsync(testBuilder.DefaultSendRequest, TestContext.Current.CancellationToken);

        var body = await ReadResponseBodyAsync(httpContext);
        body.Should().EndWith("data: [DONE]\n\n");
    }

    [Fact]
    public async Task SendMessageAsync_WritesDoneMessageEvenWithNoChunks()
    {
        var testBuilder = new TestBuilder()
            .SetupStreamingResponse([]);
        var (endpoint, httpContext) = testBuilder.Build();

        await endpoint.SendMessageAsync(testBuilder.DefaultSendRequest, TestContext.Current.CancellationToken);

        var body = await ReadResponseBodyAsync(httpContext);
        body.Should().Be("data: [DONE]\n\n");
    }

    [Fact]
    public async Task SendMessageAsync_CallsMediatorWithCorrectCommand()
    {
        var testBuilder = new TestBuilder()
            .SetupStreamingResponse([]);
        var (endpoint, _) = testBuilder.Build(userId: "user-id");

        var request = new SendMessageRequest(
            ConversationId: "550e8400-e29b-41d4-a716-446655440000",
            Message: "Test message");

        await endpoint.SendMessageAsync(request, TestContext.Current.CancellationToken);

        testBuilder.MediatorMock.Verify(
            m => m.CreateStream(
                It.Is<SendChatMessageCommand>(c =>
                    c.ConversationId == request.ConversationId &&
                    c.UserMessage == request.Message &&
                    c.UserId == "user-id"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetHistoryAsync_ReturnsOkObjectResult()
    {
        var testBuilder = new TestBuilder()
            .SetupGetHistoryResponse();
        var (endpoint, _) = testBuilder.Build();

        var result = await endpoint.GetHistoryAsync("550e8400-e29b-41d4-a716-446655440000", TestContext.Current.CancellationToken);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetHistoryAsync_ReturnsHistoryResponseInBody()
    {
        var testBuilder = new TestBuilder()
            .SetupGetHistoryResponse();
        var (endpoint, _) = testBuilder.Build();

        var result = await endpoint.GetHistoryAsync("550e8400-e29b-41d4-a716-446655440000", TestContext.Current.CancellationToken);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(testBuilder.DefaultHistoryResponse);
    }

    [Fact]
    public async Task GetHistoryAsync_CallsMediatorWithCorrectQuery()
    {
        var testBuilder = new TestBuilder()
            .SetupGetHistoryResponse();
        var (endpoint, _) = testBuilder.Build(userId: "user-abc");
        const string conversationId = "550e8400-e29b-41d4-a716-446655440000";

        await endpoint.GetHistoryAsync(conversationId, TestContext.Current.CancellationToken);

        testBuilder.MediatorMock.Verify(
            m => m.Send(
                It.Is<GetChatHistoryQuery>(q =>
                    q.ConversationId == conversationId &&
                    q.UserId == "user-abc"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetHistoryAsync_WithSubClaim_UsesSubClaimAsUserId()
    {
        var testBuilder = new TestBuilder()
            .SetupGetHistoryResponse();
        var (endpoint, _) = testBuilder.Build(userId: "sub-user", claimType: "sub");

        await endpoint.GetHistoryAsync("550e8400-e29b-41d4-a716-446655440000", TestContext.Current.CancellationToken);

        testBuilder.MediatorMock.Verify(
            m => m.Send(
                It.Is<GetChatHistoryQuery>(q => q.UserId == "sub-user"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetHistoryAsync_WithNoClaims_UsesAnonymousAsUserId()
    {
        var testBuilder = new TestBuilder()
            .SetupGetHistoryResponse();
        var (endpoint, _) = testBuilder.Build(userId: null);

        await endpoint.GetHistoryAsync("550e8400-e29b-41d4-a716-446655440000", TestContext.Current.CancellationToken);

        testBuilder.MediatorMock.Verify(
            m => m.Send(
                It.Is<GetChatHistoryQuery>(q => q.UserId == "anonymous"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static async Task<string> ReadResponseBodyAsync(DefaultHttpContext httpContext)
    {
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        return await new StreamReader(httpContext.Response.Body, Encoding.UTF8).ReadToEndAsync();
    }
}
