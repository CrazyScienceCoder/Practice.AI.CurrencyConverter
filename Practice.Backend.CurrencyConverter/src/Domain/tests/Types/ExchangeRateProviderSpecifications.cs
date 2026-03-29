using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Domain.Tests.Types;

public sealed class ExchangeRateProviderSpecifications
{
    [Fact]
    public void Frankfurter_Name_ReturnsFrankfurter()
    {
        ExchangeRateProvider.Frankfurter.Name.Should().Be("Frankfurter");
    }

    [Fact]
    public void Frankfurter_Value_ReturnsOne()
    {
        ExchangeRateProvider.Frankfurter.Value.Should().Be(1);
    }

    [Fact]
    public void List_Always_ContainsSingleProvider()
    {
        ExchangeRateProvider.List.Should().ContainSingle();
    }

    [Fact]
    public void FromName_ValidName_ReturnsFrankfurterProvider()
    {
        var provider = ExchangeRateProvider.FromName("Frankfurter");

        provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }

    [Fact]
    public void FromValue_ValidValue_ReturnsFrankfurterProvider()
    {
        var provider = ExchangeRateProvider.FromValue(1);

        provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }
}
