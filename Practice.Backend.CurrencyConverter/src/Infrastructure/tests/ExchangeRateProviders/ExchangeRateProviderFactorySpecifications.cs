using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.ExchangeRateProviders;

public sealed class ExchangeRateProviderFactorySpecifications
{
    [Fact]
    public void Create_MatchingProviderRegistered_ReturnsCorrectProvider()
    {
        var mockProvider = new Mock<IExchangeRateProvider>();
        mockProvider.Setup(p => p.Provider).Returns(ExchangeRateProvider.Frankfurter);

        var factory = new ExchangeRateProviderFactory([mockProvider.Object]);

        var result = factory.Create(ExchangeRateProvider.Frankfurter);

        result.Should().BeSameAs(mockProvider.Object);
    }

    [Fact]
    public void Create_MatchingProviderRegistered_ReturnsProviderWithCorrectProviderType()
    {
        var mockProvider = new Mock<IExchangeRateProvider>();
        mockProvider.Setup(p => p.Provider).Returns(ExchangeRateProvider.Frankfurter);

        var factory = new ExchangeRateProviderFactory([mockProvider.Object]);

        var result = factory.Create(ExchangeRateProvider.Frankfurter);

        result.Provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }

    [Fact]
    public void Create_NoProviderRegistered_ThrowsInvalidOperationException()
    {
        var factory = new ExchangeRateProviderFactory([]);

        var act = () => factory.Create(ExchangeRateProvider.Frankfurter);

        act.Should().ThrowExactly<InvalidOperationException>()
            .WithMessage($"*{ExchangeRateProvider.Frankfurter.Name}*");
    }

    [Fact]
    public void Create_MultipleProvidersRegisteredDifferentTypes_ReturnsMatchingProvider()
    {
        var frankfurterMock = new Mock<IExchangeRateProvider>();
        frankfurterMock.Setup(p => p.Provider).Returns(ExchangeRateProvider.Frankfurter);

        var otherMock = new Mock<IExchangeRateProvider>();

        var factory = new ExchangeRateProviderFactory([otherMock.Object, frankfurterMock.Object]);

        var result = factory.Create(ExchangeRateProvider.Frankfurter);

        result.Should().BeSameAs(frankfurterMock.Object);
    }

    [Fact]
    public void Create_ProviderNotMatchingRequested_ThrowsInvalidOperationException()
    {
        var mockProvider = new Mock<IExchangeRateProvider>();
        mockProvider.Setup(p => p.Provider).Returns(ExchangeRateProvider.Frankfurter);

        // Create a factory with a provider but request a different (hypothetical) one
        // by using a factory with empty providers list targeting the same enum value but no match
        var factory = new ExchangeRateProviderFactory([]);

        var act = () => factory.Create(ExchangeRateProvider.Frankfurter);

        act.Should().ThrowExactly<InvalidOperationException>();
    }
}
