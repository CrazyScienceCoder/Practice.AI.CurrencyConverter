namespace Practice.Chatbot.CurrencyConverter.Domain.Exceptions;

public sealed class InvalidUserIdException(string message, string parameterName) : DomainValidationException(message, parameterName);
