using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.Application.Shared.Mappers;
using Practice.Backend.CurrencyConverter.Domain.Exceptions;

namespace Practice.Backend.CurrencyConverter.Application.ExchangeRates.Behaviors;

public abstract class BehaviorBase<TRequest, TResponse>(ILogger logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : ResultBase, new() 
{
    protected abstract Task<TResponse> ExecuteAsync(TRequest request
        , RequestHandlerDelegate<TResponse> next
        , CancellationToken cancellationToken);

    public async Task<TResponse> Handle(TRequest request
        , RequestHandlerDelegate<TResponse> next
        , CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            return await ExecuteAsync(request, next, cancellationToken);
        }
        catch (DomainValidationException domainValidationException)
        {
            logger.LogError(domainValidationException, "Invalid inputs, behavior type: {BehaviorType}",
                GetType().Name);

            return domainValidationException.ToFailedResponse<TResponse>(ErrorType.ValidationError);
        }
        catch (Exception exception)
        {
            logger.LogError(exception,
                "Something went wrong while executing the behavior, behavior type: {HandlerType}",
                GetType().Name);

            return exception.ToFailedResponse<TResponse>(ErrorType.Generic);
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation("Behavior: {BehaviorType}, ElapsedTime {ElapsedTime} ms", GetType().Name,
                stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}