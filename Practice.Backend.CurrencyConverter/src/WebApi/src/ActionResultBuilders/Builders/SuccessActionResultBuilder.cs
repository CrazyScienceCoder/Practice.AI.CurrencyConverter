using Microsoft.AspNetCore.Mvc;
using Practice.Backend.CurrencyConverter.Application.Shared;

namespace Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Builders;

public sealed class SuccessActionResultBuilder : ActionResultBuilderBase
{
    public override bool CanHandle<TData, TResponse>(Result<TData, TResponse> result)
        => result.IsSuccess;

    public override IActionResult Build<TData, TResponse>(Result<TData, TResponse> result)
        => new OkObjectResult(result.Data);
}
