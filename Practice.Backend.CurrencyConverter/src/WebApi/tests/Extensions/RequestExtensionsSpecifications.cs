using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;
using Practice.Backend.CurrencyConverter.WebApi.Extensions;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Extensions;

public sealed class RequestExtensionsSpecifications
{
    [Fact]
    public void BuildProvider_NullProvider_ReturnsFrankfurterProvider()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "USD", Provider = null };

        var provider = request.BuildProvider();

        provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }

    [Fact]
    public void BuildProvider_FrankfurterProviderName_ReturnsFrankfurterProvider()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "USD", Provider = "Frankfurter" };

        var provider = request.BuildProvider();

        provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }

    [Fact]
    public void BuildProvider_FrankfurterProviderNameLowercase_ReturnsFrankfurterProvider()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "USD", Provider = "frankfurter" };

        var provider = request.BuildProvider();

        provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }

    [Fact]
    public void BuildProvider_FrankfurterProviderNameUppercase_ReturnsFrankfurterProvider()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "USD", Provider = "FRANKFURTER" };

        var provider = request.BuildProvider();

        provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }

    [Fact]
    public void BuildProvider_NullProvider_ReturnsFrankfurterAsDefault()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "USD", Provider = null };

        var provider = request.BuildProvider();

        provider.Name.Should().Be(ExchangeRateProvider.Frankfurter.Name);
    }
}
