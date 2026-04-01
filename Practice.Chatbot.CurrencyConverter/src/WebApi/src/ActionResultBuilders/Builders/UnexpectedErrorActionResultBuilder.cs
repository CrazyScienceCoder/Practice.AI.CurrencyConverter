using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Practice.Chatbot.CurrencyConverter.Application.Shared;
using Practice.Chatbot.CurrencyConverter.WebApi.Constants;

namespace Practice.Chatbot.CurrencyConverter.WebApi.ActionResultBuilders.Builders;

public sealed class UnexpectedErrorActionResultBuilder(
    IHttpContextAccessor httpContextAccessor,
    ProblemDetailsFactory problemDetailsFactory)
    : ProblemActionResultBuilderBase(httpContextAccessor, problemDetailsFactory)
{
    public override bool CanHandle<TData, TResponse>(Result<TData, TResponse> result)
        => !result.IsSuccess && result.ErrorType == ErrorType.Generic;

    public override IActionResult Build<TData, TResponse>(Result<TData, TResponse> result)
    {
        var problemDetails = CreateProblemDetails(status: StatusCodes.Status500InternalServerError
            , title: ResponseTitles.UnexpectedError
            , detail: result.Message
            , code: ErrorCodes.UnexpectedError);

        return new ObjectResult(value: problemDetails)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}
