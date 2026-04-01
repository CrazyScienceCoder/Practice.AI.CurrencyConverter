using System.Diagnostics;
using System.Runtime.CompilerServices;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Practice.Chatbot.CurrencyConverter.Application.Shared;

public abstract class StreamingHandlerBase<TCommand>(ILogger logger)
    : IStreamRequestHandler<TCommand, string>
    where TCommand : IStreamRequest<string>
{
    protected abstract IAsyncEnumerable<string> ExecuteAsync(TCommand command, CancellationToken cancellationToken);

    public async IAsyncEnumerable<string> Handle(
        TCommand command,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await foreach (var chunk in ExecuteAsync(command, cancellationToken).WithCancellation(cancellationToken))
            {
                yield return chunk;
            }
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation("Handler: {HandlerType}, ElapsedTime {ElapsedTime} ms", GetType().Name,
                stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}
