using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Practice.Chatbot.CurrencyConverter.WebApi.Constants;

namespace Practice.Chatbot.CurrencyConverter.WebApi.ActionResultBuilders.Builders;

public abstract class ProblemActionResultBuilderBase(
    IHttpContextAccessor httpContextAccessor,
    ProblemDetailsFactory problemDetailsFactory) : ActionResultBuilderBase
{
    protected readonly IHttpContextAccessor HttpContextAccessor = httpContextAccessor;
    protected readonly ProblemDetailsFactory ProblemDetailsFactory = problemDetailsFactory;

    protected ProblemDetails CreateProblemDetails(int status
        , string title
        , string? detail
        , string code)
    {
        var problemDetails = ProblemDetailsFactory.CreateProblemDetails(httpContext: HttpContextAccessor.HttpContext!
            , statusCode: status
            , title: title
            , detail: detail
            , instance: HttpContextAccessor.HttpContext!.Request.Path);

        foreach (var extension in BuildExtensions(code))
        {
            problemDetails.Extensions.Add(extension);
        }

        return problemDetails;
    }

    protected static Dictionary<string, object?> BuildExtensions(string code)
    {
        return new Dictionary<string, object?>
        {
            [ExtensionKeys.Code] = code,
            [ExtensionKeys.Timestamp] = DateTime.UtcNow
        };
    }
}
