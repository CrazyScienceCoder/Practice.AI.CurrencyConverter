using Microsoft.AspNetCore.Mvc;
using Practice.Chatbot.CurrencyConverter.Application.Shared;

namespace Practice.Chatbot.CurrencyConverter.WebApi.ActionResultBuilders.Builders;

public sealed class SuccessActionResultBuilder : ActionResultBuilderBase
{
    public override bool CanHandle<TData, TResponse>(Result<TData, TResponse> result)
        => result.IsSuccess;

    public override IActionResult Build<TData, TResponse>(Result<TData, TResponse> result)
        => new OkObjectResult(result.Data);
}
