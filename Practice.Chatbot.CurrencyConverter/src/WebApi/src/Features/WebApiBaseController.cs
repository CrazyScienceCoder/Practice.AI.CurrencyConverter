using Microsoft.AspNetCore.Mvc;
using Practice.Chatbot.CurrencyConverter.Application.Shared;
using Practice.Chatbot.CurrencyConverter.WebApi.ActionResultBuilders.Factories;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Features;

public abstract class WebApiBaseController(IActionResultBuilderFactory factory) : ControllerBase
{
    protected IActionResult BuildResponse<TData, TResponse>(Result<TData, TResponse> result)
        where TResponse : Result<TData, TResponse>, new()
        => factory.Create(result).Build(result);
}
