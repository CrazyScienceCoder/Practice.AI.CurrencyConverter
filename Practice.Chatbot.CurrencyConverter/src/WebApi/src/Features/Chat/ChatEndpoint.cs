using System.Text;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Practice.Chatbot.CurrencyConverter.Application.Chat.GetHistory;
using Practice.Chatbot.CurrencyConverter.WebApi.ActionResultBuilders.Factories;
using Practice.Chatbot.CurrencyConverter.WebApi.Extensions;
using Practice.Chatbot.CurrencyConverter.WebApi.Features.Chat.SendMessage;
using Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Authentication;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Features.Chat;

[Route(ChatRoutes.BasePath)]
[ApiVersion(ChatRoutes.Version.V1)]
[ApiController]
[Authorize(Policy = Policies.AiChat)]
public sealed class ChatEndpoint(IMediator mediator, IActionResultBuilderFactory factory)
    : WebApiBaseController(factory)
{
    [HttpPost(ChatRoutes.MessagePath)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task SendMessageAsync(
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var command = request.ToCommand(userId);

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
    [ProducesResponseType(type: typeof(GetChatHistoryQueryResult), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
    [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(statusCode: StatusCodes.Status403Forbidden)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHistoryAsync(
        [FromRoute] string conversationId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var query = new GetChatHistoryQuery { ConversationId = conversationId, UserId = userId };
        var result = await mediator.Send(query, cancellationToken);

        return BuildResponse(result);
    }
}
