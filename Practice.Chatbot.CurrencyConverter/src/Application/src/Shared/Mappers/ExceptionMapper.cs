namespace Practice.Chatbot.CurrencyConverter.Application.Shared.Mappers;

public static class ExceptionMapper
{
    extension(Exception exception)
    {
        public TResponse ToFailedResponse<TResponse, TData>(ErrorType errorType)
            where TResponse : Result<TData, TResponse>, new()
            => Result<TData, TResponse>.Failure(exception: exception, errorType: errorType, message: exception.Message);

        public TResponse ToFailedResponse<TResponse>(ErrorType errorType)
            where TResponse : ResultBase, new()
            => new()
            {
                Error = exception,
                ErrorType = errorType,
                IsSuccess = false,
                Message = exception.Message
            };
    }
}
