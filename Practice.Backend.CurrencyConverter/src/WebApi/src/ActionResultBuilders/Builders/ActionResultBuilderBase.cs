using Microsoft.AspNetCore.Mvc;
using Practice.Backend.CurrencyConverter.Application.Shared;

namespace Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Builders;

public abstract class ActionResultBuilderBase : IActionResultBuilder
{
    public abstract bool CanHandle<TData, TResponse>(Result<TData, TResponse> result)
        where TResponse : Result<TData, TResponse>, new();

    public abstract IActionResult Build<TData, TResponse>(Result<TData, TResponse> result)
        where TResponse : Result<TData, TResponse>, new();
}