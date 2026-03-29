using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.WebApi.Constants;

namespace Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Builders;

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
            , title: ResponseTitles.ExchangeRateNotFound
            , detail: result.Message
            , code: ErrorCodes.ExchangeRateNotFound);

        return new NotFoundObjectResult(value: problemDetails);
    }
}
