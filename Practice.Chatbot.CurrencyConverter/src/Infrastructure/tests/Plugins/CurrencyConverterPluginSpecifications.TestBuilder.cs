using System.Net;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Client;
using Practice.Backend.CurrencyConverter.Client.Exceptions;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;
using Practice.Chatbot.CurrencyConverter.Infrastructure.Plugins;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Tests.Plugins;

public sealed partial class CurrencyConverterPluginSpecifications
{
    private class TestBuilder
    {
        public Mock<ICurrencyConverterClient> ClientMock { get; } = new();
        private readonly Mock<ILogger<CurrencyConverterPlugin>> _loggerMock = new();

        private static readonly ExchangeRateResponse DefaultExchangeRateResponse = new(
            Amount: 1m,
            Base: "EUR",
            Date: new DateOnly(2024, 1, 15),
            Rates: new Dictionary<string, decimal> { ["USD"] = 1.08m });

        private static readonly HistoricalExchangeRateResponse DefaultHistoricalResponse = new(
            Amount: 1m,
            Base: "EUR",
            StartDate: new DateOnly(2024, 1, 1),
            EndDate: new DateOnly(2024, 1, 15),
            Rates: new Dictionary<DateOnly, IReadOnlyDictionary<string, decimal>>
            {
                [new DateOnly(2024, 1, 1)] = new Dictionary<string, decimal> { ["USD"] = 1.08m }
            },
            PageNumber: 1,
            HasMore: false,
            TotalNumberOfPages: 1);

        public TestBuilder WithLatestResult(ExchangeRateResponse response)
        {
            ClientMock
                .Setup(c => c.GetLatestExchangeRatesAsync(
                    It.IsAny<LatestExchangeRatesRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            return this;
        }

        public TestBuilder WithLatestException(Exception exception)
        {
            ClientMock
                .Setup(c => c.GetLatestExchangeRatesAsync(
                    It.IsAny<LatestExchangeRatesRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            return this;
        }

        public TestBuilder WithConversionResult(ExchangeRateResponse response)
        {
            ClientMock
                .Setup(c => c.GetConversionAsync(
                    It.IsAny<ConversionRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            return this;
        }

        public TestBuilder WithConversionException(Exception exception)
        {
            ClientMock
                .Setup(c => c.GetConversionAsync(
                    It.IsAny<ConversionRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            return this;
        }

        public TestBuilder WithHistoricalResult(HistoricalExchangeRateResponse response)
        {
            ClientMock
                .Setup(c => c.GetHistoricalExchangeRatesAsync(
                    It.IsAny<HistoricalExchangeRateRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            return this;
        }

        public TestBuilder WithHistoricalException(Exception exception)
        {
            ClientMock
                .Setup(c => c.GetHistoricalExchangeRatesAsync(
                    It.IsAny<HistoricalExchangeRateRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            return this;
        }

        public CurrencyConverterPlugin Build() => new(ClientMock.Object, _loggerMock.Object);

        public static ExchangeRateResponse BuildExchangeRateResponse() => DefaultExchangeRateResponse;

        public static HistoricalExchangeRateResponse BuildHistoricalResponse() => DefaultHistoricalResponse;

        public static CurrencyConverterApiException CreateApiException(HttpStatusCode statusCode)
        {
            try
            {
                CurrencyConverterApiException.ThrowIfNull<string>(null!, statusCode, "http://test/api");
                throw new InvalidOperationException("ThrowIfNull did not throw.");
            }
            catch (CurrencyConverterApiException ex)
            {
                return ex;
            }
        }
    }
}
