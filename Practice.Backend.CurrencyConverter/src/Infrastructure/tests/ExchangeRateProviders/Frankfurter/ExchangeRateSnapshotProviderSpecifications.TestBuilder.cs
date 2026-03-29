using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Clients;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Models;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Frankfurter;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.ExchangeRateProviders.Frankfurter;

public partial class ExchangeRateSnapshotProviderSpecifications
{
    private class TestBuilder
    {
        private readonly Mock<IFrankfurterApiClient> _clientMock = new();
        private readonly Mock<ILogger<ExchangeRateSnapshotProvider>> _loggerMock = new();

        public TestBuilder WithLatestResponse(LatestResponse response)
        {
            _clientMock
                .Setup(c => c.GetLatestAsync(
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            return this;
        }

        public TestBuilder WithLatestThrows(Exception exception)
        {
            _clientMock
                .Setup(c => c.GetLatestAsync(
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            return this;
        }

        public TestBuilder WithTimeSeriesResponse(TimeSeriesResponse response)
        {
            _clientMock
                .Setup(c => c.GetTimeSeriesAsync(
                    It.IsAny<DateOnly>(),
                    It.IsAny<DateOnly>(),
                    It.IsAny<string>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            return this;
        }

        public TestBuilder WithTimeSeriesThrows(Exception exception)
        {
            _clientMock
                .Setup(c => c.GetTimeSeriesAsync(
                    It.IsAny<DateOnly>(),
                    It.IsAny<DateOnly>(),
                    It.IsAny<string>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            return this;
        }

        public ExchangeRateSnapshotProvider Build()
            => new(_clientMock.Object, _loggerMock.Object);
    }
}
