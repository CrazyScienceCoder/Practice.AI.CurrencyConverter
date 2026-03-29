using Microsoft.AspNetCore.Mvc;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Factories;

namespace Practice.Backend.CurrencyConverter.WebApi.Features;

public abstract class WebApiBaseController(IActionResultBuilderFactory factory) : ControllerBase
{
    protected IActionResult BuildResponse<TData, TResponse>(Result<TData, TResponse> result)
        where TResponse : Result<TData, TResponse>, new()
        => factory.Create(result).Build(result);
}
