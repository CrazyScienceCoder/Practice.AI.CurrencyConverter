using Microsoft.AspNetCore.Mvc;
using Practice.Backend.CurrencyConverter.Application.Shared;

namespace Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Builders;

public interface IActionResultBuilder
{
    bool CanHandle<TData, TResponse>(Result<TData, TResponse> result)
        where TResponse : Result<TData, TResponse>, new();

    IActionResult Build<TData, TResponse>(Result<TData, TResponse> result)
        where TResponse : Result<TData, TResponse>, new();
}
