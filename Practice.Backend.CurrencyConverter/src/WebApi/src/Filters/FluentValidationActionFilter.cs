using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Practice.Backend.CurrencyConverter.WebApi.Filters;

public sealed class FluentValidationActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var arg in context.ActionArguments.Values)
        {
            if (arg is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());
            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var result = await validator.ValidateAsync(new ValidationContext<object>(arg));
            if (result.IsValid)
            {
                continue;
            }

            context.Result = new BadRequestObjectResult(BuildProblemDetails(context, result));
            return;
        }

        await next();
    }

    private static ValidationProblemDetails BuildProblemDetails(ActionExecutingContext context, ValidationResult result)
    {
        var modelStateDictionary = new ModelStateDictionary();

        foreach (var error in result.Errors)
        {
            modelStateDictionary.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        var problemFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();

        var problem = problemFactory.CreateValidationProblemDetails(httpContext: context.HttpContext
            , modelStateDictionary: modelStateDictionary
            , statusCode: StatusCodes.Status400BadRequest
            , instance: context.HttpContext.Request.Path);

        return problem;
    }
}
