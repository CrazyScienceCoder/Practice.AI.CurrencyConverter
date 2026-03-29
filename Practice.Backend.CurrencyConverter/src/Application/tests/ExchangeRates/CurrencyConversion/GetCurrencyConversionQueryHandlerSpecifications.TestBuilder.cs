using ErrorOr;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.CurrencyConversion;
using Practice.Backend.CurrencyConverter.Domain.Exceptions;
using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Application.Tests.ExchangeRates.CurrencyConversion;

public partial class GetCurrencyConversionQueryHandlerSpecifications
{
    private class TestBuilder
    {
        public readonly Mock<IExchangeRateProviderFactory> ProviderFactoryMock = new();
        public readonly Mock<IExchangeRateProvider> ProviderMock = new();
        private readonly Mock<ILogger<GetCurrencyConversionQueryHandler>> _loggerMock = new();

        public readonly GetCurrencyConversionQuery DefaultQuery = new()
        {
            BaseCurrency = "USD",
            ToCurrency = "EUR",
            Amount = 100m,
            Provider = ExchangeRateProvider.Frankfurter
        };

        public TestBuilder()
        {
            ProviderFactoryMock
                .Setup(f => f.Create(It.IsAny<ExchangeRateProvider>()))
                .Returns(ProviderMock.Object);
        }

        public TestBuilder SetupSuccess()
        {
            var exchangeRate = new ExchangeRate
            {
                Amount = new Amount(100m),
                Base = new Currency("USD"),
                Date = new ExchangeDate(new DateOnly(2025, 1, 15)),
                Rates = new Dictionary<Currency, Amount>
                {
                    { new Currency("EUR"), new Amount(92m) }
                }
            };

            ProviderMock
                .Setup(p => p.ConvertAsync(
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>(),
                    It.IsAny<Amount>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ErrorOr<ExchangeRate>)exchangeRate)
                .Verifiable(Times.Once);

            return this;
        }

        public TestBuilder SetupNotFoundError()
        {
            ProviderMock
                .Setup(p => p.ConvertAsync(
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>(),
                    It.IsAny<Amount>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ErrorOr<ExchangeRate>)Error.NotFound("exchange.rate.not_found", "Exchange rate not found"))
                .Verifiable(Times.Once);

            return this;
        }

        public TestBuilder SetupGenericError()
        {
            ProviderMock
                .Setup(p => p.ConvertAsync(
                    It.IsAny<Currency>(),
                    It.IsAny<Currency>(),
                    It.IsAny<Amount>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ErrorOr<ExchangeRate>)Error.Failure("exchange.rate.failure", "Exchange rate service failure"))
                .Verifiable(Times.Once);

            return this;
        }

        public TestBuilder SetupDomainValidationException()
        {
            ProviderFactoryMock
                .Setup(f => f.Create(It.IsAny<ExchangeRateProvider>()))
                .Throws(new InvalidCurrencyCodeException("Invalid currency code", nameof(DefaultQuery.BaseCurrency)));

            return this;
        }

        public TestBuilder SetupUnexpectedException()
        {
            ProviderFactoryMock
                .Setup(f => f.Create(It.IsAny<ExchangeRateProvider>()))
                .Throws(new Exception("Unexpected service failure"));

            return this;
        }

        public GetCurrencyConversionQueryHandler Build()
            => new(ProviderFactoryMock.Object, _loggerMock.Object);
    }
}
