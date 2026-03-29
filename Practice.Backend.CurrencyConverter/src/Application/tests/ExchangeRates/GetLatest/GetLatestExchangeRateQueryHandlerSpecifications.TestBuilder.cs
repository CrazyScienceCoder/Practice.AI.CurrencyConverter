using ErrorOr;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;
using Practice.Backend.CurrencyConverter.Domain.Exceptions;
using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Application.Tests.ExchangeRates.GetLatest;

public partial class GetLatestExchangeRateQueryHandlerSpecifications
{
    private class TestBuilder
    {
        public readonly Mock<IExchangeRateProviderFactory> ProviderFactoryMock = new();
        public readonly Mock<IExchangeRateProvider> ProviderMock = new();
        private readonly Mock<ILogger<GetLatestExchangeRateQueryHandler>> _loggerMock = new();

        public readonly GetLatestExchangeRateQuery DefaultQuery = new()
        {
            BaseCurrency = "USD",
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
            var exchangeRate = BuildExchangeRate(new Dictionary<Currency, Amount>
            {
                { new Currency("EUR"), new Amount(0.92m) },
                { new Currency("GBP"), new Amount(0.78m) }
            });

            ProviderMock
                .Setup(p => p.GetLatestExchangeRateAsync(
                    It.IsAny<Currency>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ErrorOr<ExchangeRate>)exchangeRate)
                .Verifiable(Times.Once);

            return this;
        }

        public TestBuilder SetupSuccessWithForbiddenCurrencies()
        {
            var exchangeRate = BuildExchangeRate(new Dictionary<Currency, Amount>
            {
                { new Currency("EUR"), new Amount(0.92m) },
                { new Currency("MXN"), new Amount(17.5m) },
                { new Currency("PLN"), new Amount(4.1m) }
            });

            ProviderMock
                .Setup(p => p.GetLatestExchangeRateAsync(
                    It.IsAny<Currency>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ErrorOr<ExchangeRate>)exchangeRate)
                .Verifiable(Times.Once);

            return this;
        }

        public TestBuilder SetupNotFoundError()
        {
            ProviderMock
                .Setup(p => p.GetLatestExchangeRateAsync(
                    It.IsAny<Currency>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ErrorOr<ExchangeRate>)Error.NotFound("exchange.rate.not_found", "Exchange rate not found"))
                .Verifiable(Times.Once);

            return this;
        }

        public TestBuilder SetupGenericError()
        {
            ProviderMock
                .Setup(p => p.GetLatestExchangeRateAsync(
                    It.IsAny<Currency>(),
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

        public GetLatestExchangeRateQueryHandler Build()
            => new(ProviderFactoryMock.Object, _loggerMock.Object);

        private static ExchangeRate BuildExchangeRate(Dictionary<Currency, Amount> rates)
            => new()
            {
                Amount = new Amount(1m),
                Base = new Currency("USD"),
                Date = new ExchangeDate(new DateOnly(2025, 1, 15)),
                Rates = rates
            };
    }
}
