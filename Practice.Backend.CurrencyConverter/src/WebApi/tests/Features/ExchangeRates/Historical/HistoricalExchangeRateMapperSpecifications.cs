using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetHistorical;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;
using Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Historical;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Features.ExchangeRates.Historical;

public sealed class HistoricalExchangeRateMapperSpecifications
{
    private static HistoricalExchangeRateRequest BuildValidRequest()
        => new()
        {
            BaseCurrency = "USD",
            From = new DateOnly(2025, 1, 1),
            To = new DateOnly(2025, 1, 31),
            Provider = null,
            PageNumber = null,
            DaysPerPage = null
        };

    [Fact]
    public void ToQuery_BaseCurrencyMappedCorrectly()
    {
        var request = BuildValidRequest();

        var query = request.ToQuery();

        query.BaseCurrency.Should().Be("USD");
    }

    [Fact]
    public void ToQuery_FromDateMappedCorrectly()
    {
        var request = BuildValidRequest();

        var query = request.ToQuery();

        query.From.Should().Be(new DateOnly(2025, 1, 1));
    }

    [Fact]
    public void ToQuery_ToDateMappedCorrectly()
    {
        var request = BuildValidRequest();

        var query = request.ToQuery();

        query.To.Should().Be(new DateOnly(2025, 1, 31));
    }

    [Fact]
    public void ToQuery_NullProvider_MapsToFrankfurterProvider()
    {
        var request = BuildValidRequest();

        var query = request.ToQuery();

        query.Provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }

    [Fact]
    public void ToQuery_NullPageNumber_MapsToNullPageNumber()
    {
        var request = BuildValidRequest() with { PageNumber = null };

        var query = request.ToQuery();

        query.PageNumber.Should().BeNull();
    }

    [Fact]
    public void ToQuery_PageNumberMappedCorrectly()
    {
        var request = BuildValidRequest() with { PageNumber = 3 };

        var query = request.ToQuery();

        query.PageNumber.Should().Be(3);
    }

    [Fact]
    public void ToQuery_NullDaysPerPage_MapsToNullDaysPerPage()
    {
        var request = BuildValidRequest() with { DaysPerPage = null };

        var query = request.ToQuery();

        query.DaysPerPage.Should().BeNull();
    }

    [Fact]
    public void ToQuery_DaysPerPageMappedCorrectly()
    {
        var request = BuildValidRequest() with { DaysPerPage = 15 };

        var query = request.ToQuery();

        query.DaysPerPage.Should().Be(15);
    }

    [Fact]
    public void ToQuery_ReturnsGetHistoricalExchangeRateQuery()
    {
        var request = BuildValidRequest();

        var query = request.ToQuery();

        query.Should().BeOfType<GetHistoricalExchangeRateQuery>();
    }
}
