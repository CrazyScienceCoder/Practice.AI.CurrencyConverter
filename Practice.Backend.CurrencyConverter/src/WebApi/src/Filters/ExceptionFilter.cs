using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Practice.Backend.CurrencyConverter.WebApi.Constants;

namespace Practice.Backend.CurrencyConverter.WebApi.Filters;

public sealed class ExceptionFilter(ProblemDetailsFactory problemDetailsFactory, ILogger<ExceptionFilter> logger)
    : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var problem = problemDetailsFactory.CreateProblemDetails(
            httpContext: context.HttpContext,
            statusCode: StatusCodes.Status500InternalServerError,
            title: ResponseTitles.UnexpectedError,
            detail: exception.Message,
            instance: context.HttpContext.Request.Path);

        context.Result = new ObjectResult(problem)
        {
            StatusCode = problem.Status
        };

        context.ExceptionHandled = true;
    }
}