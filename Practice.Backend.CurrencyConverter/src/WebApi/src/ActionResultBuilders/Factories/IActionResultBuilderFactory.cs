using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Builders;

namespace Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Factories;

public interface IActionResultBuilderFactory
{
    IActionResultBuilder Create<TData, TResponse>(Result<TData, TResponse> result)
        where TResponse : Result<TData, TResponse>, new();
}