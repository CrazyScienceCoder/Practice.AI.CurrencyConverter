using System.Text;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Practice.Chatbot.CurrencyConverter.Application.Chat.GetHistory;
using Practice.Chatbot.CurrencyConverter.Application.Chat.Send;
using Practice.Chatbot.CurrencyConverter.WebApi.Extensions;
using Practice.Chatbot.CurrencyConverter.WebApi.Features.Chat.SendMessage;
using Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Authentication;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Features.Chat;

[Route(ChatRoutes.BasePath)]
[ApiVersion(ChatRoutes.Version.V1)]
[ApiController]
[Authorize(Policy = Policies.AiChat)]
public sealed class ChatEndpoint(IMediator mediator) : ControllerBase
{
    [HttpPost(ChatRoutes.MessagePath)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task SendMessageAsync(
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new SendChatMessageCommand(
            ConversationId: request.ConversationId,
            UserId: userId,
            UserMessage: request.Message);

        Response.Headers["Content-Type"] = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["X-Accel-Buffering"] = "no";

        await foreach (var chunk in mediator.CreateStream(command, cancellationToken))
        {
            var escapedChunk = chunk
                .Replace("\n", "\\n")
                .Replace("\r", "\\r");

            var sseMessage = $"data: {escapedChunk}\n\n";
            await Response.WriteAsync(sseMessage, Encoding.UTF8, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }

        await Response.WriteAsync("data: [DONE]\n\n", Encoding.UTF8, cancellationToken);
        await Response.Body.FlushAsync(cancellationToken);
    }

    [HttpGet(ChatRoutes.HistoryPath)]
    [ProducesResponseType(typeof(GetChatHistoryQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetHistoryAsync(
        [FromRoute] string conversationId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var query = new GetChatHistoryQuery(conversationId, userId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
