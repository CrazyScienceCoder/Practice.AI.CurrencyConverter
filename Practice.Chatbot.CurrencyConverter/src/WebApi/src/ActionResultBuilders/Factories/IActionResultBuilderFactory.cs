using Practice.Chatbot.CurrencyConverter.Application.Shared;
using Practice.Chatbot.CurrencyConverter.WebApi.ActionResultBuilders.Builders;

namespace Practice.Chatbot.CurrencyConverter.WebApi.ActionResultBuilders.Factories;

public interface IActionResultBuilderFactory
{
    IActionResultBuilder Create<TData, TResponse>(Result<TData, TResponse> result)
        where TResponse : Result<TData, TResponse>, new();
}
