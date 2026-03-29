using ErrorOr;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Frankfurter;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.ExchangeRateProviders.Frankfurter;

public partial class FrankfurterExchangeRateProviderSpecifications
{
    private class TestBuilder
    {
        public Mock<IExchangeRateSnapshotProvider> SnapshotProviderMock { get; } = new();
        private readonly Mock<IExchangeRateSnapshotProviderFactory> _factoryMock = new();

        public TestBuilder()
        {
            SnapshotProviderMock
                .Setup(p => p.Provider)
                .Returns(ExchangeRateProvider.Frankfurter);

            _factoryMock
                .Setup(f => f.GetProvider(It.IsAny<ExchangeRateProvider>()))
                .Returns(SnapshotProviderMock.Object);
        }

        public TestBuilder WithLatestResult(ErrorOr<ExchangeRateSnapshot> result)
        {
            SnapshotProviderMock
                .Setup(p => p.GetLatestAsync(It.IsAny<Currency>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            return this;
        }

        public TestBuilder WithTimeSeriesResult(ErrorOr<HistoricalExchangeRateSnapshot> result)
        {
            SnapshotProviderMock
                .Setup(p => p.GetTimeSeriesAsync(
                    It.IsAny<Currency>(),
                    It.IsAny<ExchangeDate>(),
                    It.IsAny<ExchangeDate>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            return this;
        }

        public FrankfurterExchangeRateProvider Build()
            => new(_factoryMock.Object);
    }
}
