namespace Practice.Chatbot.CurrencyConverter.Domain.Exceptions;

public sealed class InvalidMessageContentException(string message, string parameterName) : DomainValidationException(message, parameterName);
