using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Application.Shared.Mappers;
using Practice.Backend.CurrencyConverter.Domain.Exceptions;

namespace Practice.Backend.CurrencyConverter.Application.Shared;

public abstract class HandlerBase<TCommand, TResponse, TData>(ILogger logger)
    : IRequestHandler<TCommand, TResponse> where TResponse : Result<TData, TResponse>, new()
    where TCommand : IRequest<TResponse>
{
    protected abstract Task<TResponse> ExecuteAsync(TCommand request, CancellationToken cancellationToken);

    public async Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            return await ExecuteAsync(request, cancellationToken);
        }
        catch (DomainValidationException domainValidationException)
        {
            logger.LogError(domainValidationException, "Invalid inputs, handler type: {HandlerType}",
                GetType().Name);

            return domainValidationException.ToFailedResponse<TResponse, TData>(ErrorType.ValidationError);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Something went wrong while executing the handler, handler type: {HandlerType}",
                GetType().Name);

            return exception.ToFailedResponse<TResponse, TData>(ErrorType.Generic);
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation("Handler: {HandlerType}, ElapsedTime {ElapsedTime} ms", GetType().Name,
                stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}