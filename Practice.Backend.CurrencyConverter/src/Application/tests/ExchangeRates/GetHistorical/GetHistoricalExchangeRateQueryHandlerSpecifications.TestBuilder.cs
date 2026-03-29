using ErrorOr;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetHistorical;
using Practice.Backend.CurrencyConverter.Domain.Exceptions;
using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Application.Tests.ExchangeRates.GetHistorical;

public partial class GetHistoricalExchangeRateQueryHandlerSpecifications
{
    private class TestBuilder
    {
        public readonly Mock<IExchangeRateProviderFactory> ProviderFactoryMock = new();
        public readonly Mock<IExchangeRateProvider> ProviderMock = new();
        private readonly Mock<ILogger<GetHistoricalExchangeRateQueryHandler>> _loggerMock = new();

        // Mon 2025-01-13 → Fri 2025-01-17 = 5 business days, fits in 1 page of 10
        public readonly GetHistoricalExchangeRateQuery DefaultQuery = new()
        {
            BaseCurrency = "USD",
            Provider = ExchangeRateProvider.Frankfurter,
            From = new DateOnly(2025, 1, 13),
            To = new DateOnly(2025, 1, 17)
        };

        // 1 business day (Mon), PageNumber = 2 → empty skip → NotFound
        public readonly GetHistoricalExchangeRateQuery PageOutOfRangeQuery = new()
        {
            BaseCurrency = "USD",
            Provider = ExchangeRateProvider.Frankfurter,
            From = new DateOnly(2025, 1, 13),
            To = new DateOnly(2025, 1, 13),
            PageNumber = 2,
            DaysPerPage = 10
        };

        // Sat-Sun only → no business days → NotFound
        public readonly GetHistoricalExchangeRateQuery WeekendOnlyQuery = new()
        {
            BaseCurrency = "USD",
            Provider = ExchangeRateProvider.Frankfurter,
            From = new DateOnly(2025, 1, 18),
            To = new DateOnly(2025, 1, 19)
        };

        // Mon 2025-01-13 → Fri 2025-01-24 = 10 business days, DaysPerPage = 5 → 2 pages
        public readonly GetHistoricalExchangeRateQuery PaginatedQueryPage1 = new()
        {
            BaseCurrency = "USD",
            Provider = ExchangeRateProvider.Frankfurter,
            From = new DateOnly(2025, 1, 13),
            To = new DateOnly(2025, 1, 24),
            PageNumber = 1,
            DaysPerPage = 5
        };

        public readonly GetHistoricalExchangeRateQuery PaginatedQueryPage2 = new()
        {
            BaseCurrency = "USD",
            Provider = ExchangeRateProvider.Frankfurter,
            From = new DateOnly(2025, 1, 13),
            To = new DateOnly(2025, 1, 24),
            PageNumber = 2,
            DaysPerPage = 5
        };

        public TestBuilder()
        {
            ProviderFactoryMock
                .Setup(f => f.Create(It.IsAny<ExchangeRateProvider>()))
                .Returns(ProviderMock.Object);
        }

        public TestBuilder SetupSuccess()
        {
            var historicalRate = BuildHistoricalExchangeRate(
                from: new DateOnly(2025, 1, 13),
                to: new DateOnly(2025, 1, 17),
                dailyRates: new Dictionary<Currency, Amount>
                {
                    { new Currency("EUR"), new Amount(0.92m) }
                });

            ProviderMock
                .Setup(p => p.GetHistoricalExchangeRateAsync(
                    It.IsAny<Currency>(),
                    It.IsAny<ExchangeDate>(),
                    It.IsAny<ExchangeDate>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ErrorOr<HistoricalExchangeRate>)historicalRate)
                .Verifiable(Times.Once);

            return this;
        }

        public TestBuilder SetupSuccessForPaginatedQuery()
        {
            var historicalRate = BuildHistoricalExchangeRate(
                from: new DateOnly(2025, 1, 13),
                to: new DateOnly(2025, 1, 17),
                dailyRates: new Dictionary<Currency, Amount>
                {
                    { new Currency("EUR"), new Amount(0.92m) }
                });

            ProviderMock
                .Setup(p => p.GetHistoricalExchangeRateAsync(
                    It.IsAny<Currency>(),
                    It.IsAny<ExchangeDate>(),
                    It.IsAny<ExchangeDate>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ErrorOr<HistoricalExchangeRate>)historicalRate)
                .Verifiable(Times.Once);

            return this;
        }

        public TestBuilder SetupSuccessWithForbiddenCurrencies()
        {
            var historicalRate = BuildHistoricalExchangeRate(
                from: new DateOnly(2025, 1, 13),
                to: new DateOnly(2025, 1, 17),
                dailyRates: new Dictionary<Currency, Amount>
                {
                    { new Currency("EUR"), new Amount(0.92m) },
                    { new Currency("MXN"), new Amount(17.5m) },
                    { new Currency("PLN"), new Amount(4.1m) }
                });

            ProviderMock
                .Setup(p => p.GetHistoricalExchangeRateAsync(
                    It.IsAny<Currency>(),
                    It.IsAny<ExchangeDate>(),
                    It.IsAny<ExchangeDate>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ErrorOr<HistoricalExchangeRate>)historicalRate)
                .Verifiable(Times.Once);

            return this;
        }

        public TestBuilder SetupPageOutOfRange()
        {
            // Provider is never reached when pageDates is empty, so no provider setup needed
            return this;
        }

        public TestBuilder SetupNotFoundError()
        {
            ProviderMock
                .Setup(p => p.GetHistoricalExchangeRateAsync(
                    It.IsAny<Currency>(),
                    It.IsAny<ExchangeDate>(),
                    It.IsAny<ExchangeDate>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ErrorOr<HistoricalExchangeRate>)Error.NotFound("historical.rate.not_found", "Historical rate not found"))
                .Verifiable(Times.Once);

            return this;
        }

        public TestBuilder SetupGenericError()
        {
            ProviderMock
                .Setup(p => p.GetHistoricalExchangeRateAsync(
                    It.IsAny<Currency>(),
                    It.IsAny<ExchangeDate>(),
                    It.IsAny<ExchangeDate>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ErrorOr<HistoricalExchangeRate>)Error.Failure("historical.rate.failure", "Historical rate service failure"))
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

        public GetHistoricalExchangeRateQueryHandler Build()
            => new(ProviderFactoryMock.Object, _loggerMock.Object);

        private static HistoricalExchangeRate BuildHistoricalExchangeRate(
            DateOnly from,
            DateOnly to,
            Dictionary<Currency, Amount> dailyRates)
        {
            var rates = new Dictionary<ExchangeDate, Dictionary<Currency, Amount>>();

            for (var date = from; date <= to; date = date.AddDays(1))
            {
                if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                    continue;

                rates[new ExchangeDate(date)] = new Dictionary<Currency, Amount>(dailyRates);
            }

            return new HistoricalExchangeRate
            {
                Amount = new Amount(1m),
                Base = new Currency("USD"),
                StartDate = new ExchangeDate(from),
                EndDate = new ExchangeDate(to),
                Rates = rates
            };
        }
    }
}
