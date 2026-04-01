using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Practice.Chatbot.CurrencyConverter.Application.Shared;
using Practice.Chatbot.CurrencyConverter.WebApi.Constants;

namespace Practice.Chatbot.CurrencyConverter.WebApi.ActionResultBuilders.Builders;

public sealed class NotFoundActionResultBuilder(
    IHttpContextAccessor httpContextAccessor,
    ProblemDetailsFactory problemDetailsFactory)
    : ProblemActionResultBuilderBase(httpContextAccessor, problemDetailsFactory)
{
    public override bool CanHandle<TData, TResponse>(Result<TData, TResponse> result)
        => !result.IsSuccess && result.ErrorType == ErrorType.NotFound;

    public override IActionResult Build<TData, TResponse>(Result<TData, TResponse> result)
    {
        var problemDetails = CreateProblemDetails(status: StatusCodes.Status404NotFound
            , title: ResponseTitles.ChatHistoryNotFound
            , detail: result.Message
            , code: ErrorCodes.ChatHistoryNotFound);

        return new NotFoundObjectResult(value: problemDetails);
    }
}
