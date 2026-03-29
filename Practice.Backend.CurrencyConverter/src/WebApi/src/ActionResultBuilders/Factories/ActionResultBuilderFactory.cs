using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Builders;

namespace Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Factories;

public sealed class ActionResultBuilderFactory(IEnumerable<IActionResultBuilder> builders) : IActionResultBuilderFactory
{
    public IActionResultBuilder Create<TData, TResponse>(Result<TData, TResponse> result)
        where TResponse : Result<TData, TResponse>, new() => builders.First(b => b.CanHandle(result));
}
