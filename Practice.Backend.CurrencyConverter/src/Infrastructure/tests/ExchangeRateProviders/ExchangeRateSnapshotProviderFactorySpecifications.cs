using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.ExchangeRateProviders;

public sealed class ExchangeRateSnapshotProviderFactorySpecifications
{
    [Fact]
    public void GetProvider_MatchingProviderRegistered_ReturnsCorrectProvider()
    {
        var mockProvider = new Mock<IExchangeRateSnapshotProvider>();
        mockProvider.Setup(p => p.Provider).Returns(ExchangeRateProvider.Frankfurter);

        var factory = new ExchangeRateSnapshotProviderFactory([mockProvider.Object]);

        var result = factory.GetProvider(ExchangeRateProvider.Frankfurter);

        result.Should().BeSameAs(mockProvider.Object);
    }

    [Fact]
    public void GetProvider_MatchingProviderRegistered_ReturnsProviderWithCorrectProviderType()
    {
        var mockProvider = new Mock<IExchangeRateSnapshotProvider>();
        mockProvider.Setup(p => p.Provider).Returns(ExchangeRateProvider.Frankfurter);

        var factory = new ExchangeRateSnapshotProviderFactory([mockProvider.Object]);

        var result = factory.GetProvider(ExchangeRateProvider.Frankfurter);

        result.Provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }

    [Fact]
    public void GetProvider_NoProviderRegistered_ThrowsInvalidOperationException()
    {
        var factory = new ExchangeRateSnapshotProviderFactory([]);

        var act = () => factory.GetProvider(ExchangeRateProvider.Frankfurter);

        act.Should().ThrowExactly<InvalidOperationException>()
            .WithMessage($"*{ExchangeRateProvider.Frankfurter}*");
    }

    [Fact]
    public void GetProvider_MultipleProvidersRegistered_ReturnsMatchingProvider()
    {
        var frankfurterMock = new Mock<IExchangeRateSnapshotProvider>();
        frankfurterMock.Setup(p => p.Provider).Returns(ExchangeRateProvider.Frankfurter);

        var otherMock = new Mock<IExchangeRateSnapshotProvider>();

        var factory = new ExchangeRateSnapshotProviderFactory([otherMock.Object, frankfurterMock.Object]);

        var result = factory.GetProvider(ExchangeRateProvider.Frankfurter);

        result.Should().BeSameAs(frankfurterMock.Object);
    }

    [Fact]
    public void GetProvider_ProviderNotRegistered_ThrowsInvalidOperationExceptionWithProviderInfo()
    {
        var factory = new ExchangeRateSnapshotProviderFactory([]);

        var act = () => factory.GetProvider(ExchangeRateProvider.Frankfurter);

        act.Should().ThrowExactly<InvalidOperationException>();
    }
}
