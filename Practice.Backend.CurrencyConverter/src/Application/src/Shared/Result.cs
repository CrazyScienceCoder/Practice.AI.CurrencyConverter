namespace Practice.Backend.CurrencyConverter.Application.Shared;

public abstract class ResultBase
{
    public bool IsSuccess { get; init; }

    public string? Message { get; init; }

    public Exception? Error { get; init; }

    public ErrorType? ErrorType { get; init; }
}

public abstract class Result<TData, TResponse> : ResultBase where TResponse : Result<TData, TResponse>, new()
{
    private const string DefaultSuccessMessage = "Successfully executed";
    private const string DefaultFailureMessage = "Failed to execute";

    public TData? Data { get; private init; }

    public static TResponse Success(TData? data = default, string message = DefaultSuccessMessage)
    {
        return new TResponse
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }

    public static TResponse Failure(Exception? exception = null, ErrorType? errorType = null, string message = DefaultFailureMessage)
    {
        return new TResponse
        {
            IsSuccess = false,
            Message = message,
            Error = exception,
            ErrorType = errorType
        };
    }
}