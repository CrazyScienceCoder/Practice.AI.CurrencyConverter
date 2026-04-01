using Practice.Chatbot.CurrencyConverter.Application.Shared;

namespace Practice.Chatbot.CurrencyConverter.Application.Tests.Chat.GetHistory;

public partial class GetChatHistoryQueryHandlerSpecifications
{
    [Fact]
    public async Task Handle_ConversationFoundAndUserMatches_ReturnsSuccessResponse()
    {
        var testBuilder = new TestBuilder()
            .SetupConversationFound();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.RepositoryMock.Verify();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ConversationId.Should().Be(testBuilder.DefaultQuery.ConversationId);
        result.Data.Messages.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ConversationFound_MapsUserMessageRoleToLowerCase()
    {
        var testBuilder = new TestBuilder()
            .SetupConversationFound();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.RepositoryMock.Verify();
        result.Data!.Messages[0].Role.Should().Be("user");
    }

    [Fact]
    public async Task Handle_ConversationFound_MapsAssistantMessageRoleToLowerCase()
    {
        var testBuilder = new TestBuilder()
            .SetupConversationFound();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.RepositoryMock.Verify();
        result.Data!.Messages[1].Role.Should().Be("assistant");
    }

    [Fact]
    public async Task Handle_ConversationFound_MapsMessageContentAndTimestamp()
    {
        var testBuilder = new TestBuilder()
            .SetupConversationFound();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.RepositoryMock.Verify();
        result.Data!.Messages[0].Content.Should().Be("What is the exchange rate for USD?");
        result.Data.Messages[0].Timestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_ConversationFoundWithNoMessages_ReturnsSuccessWithEmptyMessages()
    {
        var testBuilder = new TestBuilder()
            .SetupConversationFoundWithNoMessages();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.RepositoryMock.Verify();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Messages.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ConversationNotFound_ReturnsNotFoundResponse()
    {
        var testBuilder = new TestBuilder()
            .SetupConversationNotFound();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.RepositoryMock.Verify();
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ConversationBelongsToDifferentUser_ReturnsNotFoundResponse()
    {
        var testBuilder = new TestBuilder()
            .SetupConversationBelongsToDifferentUser();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.RepositoryMock.Verify();
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_InvalidConversationId_ReturnsValidationErrorResponse()
    {
        var handler = new TestBuilder().Build();
        var query = new Application.Chat.GetHistory.GetChatHistoryQuery
        {
            ConversationId = "not-a-valid-guid",
            UserId = "user-123"
        };

        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.ValidationError);
    }
}
