namespace Practice.Chatbot.CurrencyConverter.Domain.Exceptions;

public sealed class InvalidConversationIdException(string message, string parameterName) : DomainValidationException(message, parameterName);
