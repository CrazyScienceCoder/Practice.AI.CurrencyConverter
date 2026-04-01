using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Practice.Chatbot.CurrencyConverter.Application.Shared;
using Practice.Chatbot.CurrencyConverter.Domain.Exceptions;
using Practice.Chatbot.CurrencyConverter.WebApi.Constants;

namespace Practice.Chatbot.CurrencyConverter.WebApi.ActionResultBuilders.Builders;

public sealed class ValidationErrorActionResultBuilder(
    IHttpContextAccessor httpContextAccessor,
    ProblemDetailsFactory problemDetailsFactory)
    : ProblemActionResultBuilderBase(httpContextAccessor, problemDetailsFactory)
{
    public override bool CanHandle<TData, TResponse>(Result<TData, TResponse> result)
        => !result.IsSuccess && result.ErrorType == ErrorType.ValidationError;

    public override IActionResult Build<TData, TResponse>(Result<TData, TResponse> result)
    {
        if (result.Error is DomainValidationException domainValidationException)
        {
            return new BadRequestObjectResult(CreateValidationProblem(paramName: domainValidationException.ParamName
                , message: domainValidationException.Message
                , code: ErrorCodes.DomainValidationFailed));
        }

        return new BadRequestObjectResult(CreateValidationProblem(paramName: null
            , message: result.Error?.Message ?? Constants.Constants.ValidationError
            , code: ErrorCodes.GeneralValidationFailed));
    }

    private ValidationProblemDetails CreateValidationProblem(string? paramName, string message, string code)
    {
        var modelStateDictionary = new ModelStateDictionary();

        modelStateDictionary.AddModelError(key: paramName ?? Constants.Constants.NonFieldError, errorMessage: message);

        var problemDetails = ProblemDetailsFactory.CreateValidationProblemDetails(httpContext: HttpContextAccessor.HttpContext!
            , modelStateDictionary: modelStateDictionary
            , statusCode: StatusCodes.Status400BadRequest
            , instance: HttpContextAccessor.HttpContext!.Request.Path);

        foreach (var extension in BuildExtensions(code))
        {
            problemDetails.Extensions.Add(extension);
        }

        return problemDetails;
    }
}
