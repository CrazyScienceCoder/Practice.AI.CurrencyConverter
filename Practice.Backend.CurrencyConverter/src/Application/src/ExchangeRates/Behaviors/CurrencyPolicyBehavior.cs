using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.Shared;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.Domain.CurrencyPolicy;
using ErrorType = Practice.Backend.CurrencyConverter.Application.Shared.ErrorType;

namespace Practice.Backend.CurrencyConverter.Application.ExchangeRates.Behaviors;

public sealed class CurrencyPolicyBehavior<TRequest, TResponse>(
    ICurrencyPolicy currencyPolicy,
    ILogger<CurrencyPolicyBehavior<TRequest, TResponse>> logger)
    : BehaviorBase<TRequest, TResponse>(logger)
    where TRequest : IHaveCurrencies
    where TResponse : ResultBase, new()
{
    protected override async Task<TResponse> ExecuteAsync(TRequest request
        , RequestHandlerDelegate<TResponse> next
        , CancellationToken cancellationToken)
    {
        var errors = new List<Error>();

        foreach (var currency in request.GetCurrencies())
        {
            var result = currencyPolicy.EnsureAllowed(currency);

            if (!result.IsError)
            {
                continue;
            }

            errors.AddRange(result.Errors);
        }

        if (errors.Count == 0)
        {
            return await next(cancellationToken);
        }

        var description = string.Join(", ", errors.Select(r => r.Description));

        return new TResponse
        {
            ErrorType = ErrorType.NotAllowed,
            Message = description,
            IsSuccess = false
        };
    }
}
