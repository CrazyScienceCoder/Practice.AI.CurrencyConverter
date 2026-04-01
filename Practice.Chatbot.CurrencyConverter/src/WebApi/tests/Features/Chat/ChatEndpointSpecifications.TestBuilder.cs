using System.Runtime.CompilerServices;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Practice.Chatbot.CurrencyConverter.Application.Chat.GetHistory;
using Practice.Chatbot.CurrencyConverter.Application.Chat.Send;
using Practice.Chatbot.CurrencyConverter.WebApi.Features.Chat;
using Practice.Chatbot.CurrencyConverter.WebApi.Features.Chat.SendMessage;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Tests.Features.Chat;

public partial class ChatEndpointSpecifications
{
    private sealed class TestBuilder
    {
        public readonly Mock<IMediator> MediatorMock = new();

        public readonly SendMessageRequest DefaultSendRequest = new(
            ConversationId: "550e8400-e29b-41d4-a716-446655440000",
            Message: "Hello world"
        );

        public readonly GetChatHistoryQueryResult DefaultHistoryResponse = new()
        {
            ConversationId = "550e8400-e29b-41d4-a716-446655440000",
            Messages = [new ChatMessageDto { Role = "user", Content = "Hello world", Timestamp = DateTimeOffset.UtcNow }]
        };

        public TestBuilder SetupStreamingResponse(IEnumerable<string> chunks)
        {
            MediatorMock
                .Setup(m => m.CreateStream(
                    It.IsAny<SendChatMessageCommand>(),
                    It.IsAny<CancellationToken>()))
                .Returns<SendChatMessageCommand, CancellationToken>(
                    (_, ct) => CreateAsyncEnumerable(chunks, ct));
            return this;
        }

        public TestBuilder SetupGetHistoryResponse()
        {
            MediatorMock
                .Setup(m => m.Send(It.IsAny<GetChatHistoryQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetChatHistoryQueryResponse.Success(data: DefaultHistoryResponse));
            return this;
        }

        public (ChatEndpoint endpoint, DefaultHttpContext httpContext) Build(
            string? userId = "user-123",
            string claimType = ClaimTypes.NameIdentifier)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            httpContext.Request.Path = "/api/v1/chat/message";

            if (userId is not null)
            {
                var claims = new[] { new Claim(claimType, userId) };
                var identity = new ClaimsIdentity(claims, "test");
                httpContext.User = new ClaimsPrincipal(identity);
            }
            else
            {
                httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            }

            var endpoint = new ChatEndpoint(MediatorMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext,
                    RouteData = new RouteData()
                }
            };

            return (endpoint, httpContext);
        }

        private static async IAsyncEnumerable<string> CreateAsyncEnumerable(
            IEnumerable<string> items,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
                await Task.Yield();
            }
        }
    }
}
