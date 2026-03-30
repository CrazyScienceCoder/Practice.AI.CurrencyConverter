using System.Net;
using System.Reflection;
using System.Text.Json;
using Practice.Backend.CurrencyConverter.Client.Exceptions;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;

namespace Practice.Backend.CurrencyConverter.Client.Tests.Clients;

public partial class CurrencyConverterClientSpecifications
{
    private const string ValidJsonResponse =
        """{"amount":1,"base":"EUR","date":"2024-01-15","rates":{"USD":1.08,"GBP":0.86}}""";

    [Fact]
    public async Task GetLatestExchangeRatesAsync_SuccessfulResponse_ReturnsExchangeRateResponse()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidJsonResponse);
        var client = testBuilder.Build();

        var result = await client.GetLatestExchangeRatesAsync(
            new LatestExchangeRatesRequest { BaseCurrency = "EUR" }, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.Base.Should().Be("EUR");
        result.Amount.Should().Be(1);
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_SuccessfulResponse_DeserializesRates()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidJsonResponse);
        var client = testBuilder.Build();

        var result = await client.GetLatestExchangeRatesAsync(
            new LatestExchangeRatesRequest { BaseCurrency = "EUR" }, TestContext.Current.CancellationToken);

        result.Rates.Should().ContainKey("USD");
        result.Rates.Should().ContainKey("GBP");
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_SuccessfulResponse_DeserializesDateOnly()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidJsonResponse);
        var client = testBuilder.Build();

        var result = await client.GetLatestExchangeRatesAsync(
            new LatestExchangeRatesRequest { BaseCurrency = "EUR" }, TestContext.Current.CancellationToken);

        result.Date.Should().Be(new DateOnly(2024, 1, 15));
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_Always_SendsRequestToLatestEndpoint()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidJsonResponse);
        var client = testBuilder.Build();

        await client.GetLatestExchangeRatesAsync(new LatestExchangeRatesRequest { BaseCurrency = "EUR" }, TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("latest");
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_Always_BuildsCorrectApiVersionedEndpoint()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidJsonResponse);
        var client = testBuilder.Build();

        await client.GetLatestExchangeRatesAsync(new LatestExchangeRatesRequest { BaseCurrency = "EUR" }, TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("api/v1/exchange-rate/latest");
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_WithBaseCurrency_IncludesBaseCurrencyInQuery()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidJsonResponse);
        var client = testBuilder.Build();

        await client.GetLatestExchangeRatesAsync(new LatestExchangeRatesRequest { BaseCurrency = "USD" }, TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("BaseCurrency=USD");
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_WithProvider_IncludesProviderInQuery()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidJsonResponse);
        var client = testBuilder.Build();

        await client.GetLatestExchangeRatesAsync(
            new LatestExchangeRatesRequest { BaseCurrency = "EUR", Provider = "frankfurter" }, TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("Provider=frankfurter");
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_ErrorResponse_ThrowsCurrencyConverterApiException()
    {
        var testBuilder = new TestBuilder().WithErrorResponse(HttpStatusCode.NotFound, "not found");
        var client = testBuilder.Build();

        var act = () => client.GetLatestExchangeRatesAsync(
            new LatestExchangeRatesRequest { BaseCurrency = "EUR" }, TestContext.Current.CancellationToken);

        await act.Should().ThrowExactlyAsync<CurrencyConverterApiException>();
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_ErrorResponse_ExceptionContainsStatusCode()
    {
        var testBuilder = new TestBuilder()
            .WithErrorResponse(HttpStatusCode.ServiceUnavailable, "service unavailable");
        var client = testBuilder.Build();

        var act = () => client.GetLatestExchangeRatesAsync(
            new LatestExchangeRatesRequest { BaseCurrency = "EUR" }, TestContext.Current.CancellationToken);

        (await act.Should().ThrowExactlyAsync<CurrencyConverterApiException>())
            .Which.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_NullJsonPayload_ThrowsCurrencyConverterApiException()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse("null");
        var client = testBuilder.Build();

        var act = () => client.GetLatestExchangeRatesAsync(
            new LatestExchangeRatesRequest { BaseCurrency = "EUR" }, TestContext.Current.CancellationToken);

        await act.Should().ThrowExactlyAsync<CurrencyConverterApiException>();
    }

    private const string ValidHistoricalJsonResponse =
        """{"amount":1,"base":"EUR","startDate":"2024-01-01","endDate":"2024-01-15","rates":{"2024-01-15":{"USD":1.08,"GBP":0.86}},"pageNumber":1,"hasMore":false,"totalNumberOfPages":1}""";

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_SuccessfulResponse_ReturnsHistoricalExchangeRateResponse()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidHistoricalJsonResponse);
        var client = testBuilder.Build();

        var result = await client.GetHistoricalExchangeRatesAsync(
            new HistoricalExchangeRateRequest { BaseCurrency = "EUR" }, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.Base.Should().Be("EUR");
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_Always_SendsRequestToHistoricalEndpoint()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidHistoricalJsonResponse);
        var client = testBuilder.Build();

        await client.GetHistoricalExchangeRatesAsync(
            new HistoricalExchangeRateRequest { BaseCurrency = "EUR" }, TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("historical");
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_Always_BuildsCorrectApiVersionedEndpoint()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidHistoricalJsonResponse);
        var client = testBuilder.Build();

        await client.GetHistoricalExchangeRatesAsync(
            new HistoricalExchangeRateRequest { BaseCurrency = "EUR" }, TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("api/v1/exchange-rate/historical");
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_WithBaseCurrency_IncludesBaseCurrencyInQuery()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidHistoricalJsonResponse);
        var client = testBuilder.Build();

        await client.GetHistoricalExchangeRatesAsync(
            new HistoricalExchangeRateRequest { BaseCurrency = "GBP" }, TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("BaseCurrency=GBP");
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_WithDateRange_IncludesDateRangeInQuery()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidHistoricalJsonResponse);
        var client = testBuilder.Build();

        await client.GetHistoricalExchangeRatesAsync(new HistoricalExchangeRateRequest
        {
            BaseCurrency = "EUR",
            From = new DateOnly(2024, 1, 1),
            To = new DateOnly(2024, 1, 31)
        }, TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("From=2024-01-01");
        testBuilder.LastRequestPathAndQuery.Should().Contain("To=2024-01-31");
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_WithPagination_IncludesPaginationInQuery()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidHistoricalJsonResponse);
        var client = testBuilder.Build();

        await client.GetHistoricalExchangeRatesAsync(new HistoricalExchangeRateRequest
        {
            BaseCurrency = "EUR",
            PageNumber = 2,
            DaysPerPage = 7
        }, TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("PageNumber=2");
        testBuilder.LastRequestPathAndQuery.Should().Contain("DaysPerPage=7");
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_ErrorResponse_ThrowsCurrencyConverterApiException()
    {
        var testBuilder = new TestBuilder().WithErrorResponse(HttpStatusCode.BadRequest, "bad request");
        var client = testBuilder.Build();

        var act = () => client.GetHistoricalExchangeRatesAsync(
            new HistoricalExchangeRateRequest { BaseCurrency = "EUR" }, TestContext.Current.CancellationToken);

        await act.Should().ThrowExactlyAsync<CurrencyConverterApiException>();
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_NullJsonPayload_ThrowsCurrencyConverterApiException()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse("null");
        var client = testBuilder.Build();

        var act = () => client.GetHistoricalExchangeRatesAsync(
            new HistoricalExchangeRateRequest { BaseCurrency = "EUR" }, TestContext.Current.CancellationToken);

        await act.Should().ThrowExactlyAsync<CurrencyConverterApiException>();
    }

    [Fact]
    public async Task GetConversionAsync_SuccessfulResponse_ReturnsExchangeRateResponse()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidJsonResponse);
        var client = testBuilder.Build();

        var result = await client.GetConversionAsync(
            new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 }, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.Base.Should().Be("EUR");
    }

    [Fact]
    public async Task GetConversionAsync_Always_SendsRequestToConversionEndpoint()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidJsonResponse);
        var client = testBuilder.Build();

        await client.GetConversionAsync(
            new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 }, TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("conversion");
    }

    [Fact]
    public async Task GetConversionAsync_Always_BuildsCorrectApiVersionedEndpoint()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidJsonResponse);
        var client = testBuilder.Build();

        await client.GetConversionAsync(
            new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 }, TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("api/v1/exchange-rate/conversion");
    }

    [Fact]
    public async Task GetConversionAsync_WithParameters_IncludesAllParametersInQuery()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidJsonResponse);
        var client = testBuilder.Build();

        await client.GetConversionAsync(
            new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 }, TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("BaseCurrency=EUR");
        testBuilder.LastRequestPathAndQuery.Should().Contain("ToCurrency=USD");
        testBuilder.LastRequestPathAndQuery.Should().Contain("Amount=100");
    }

    [Fact]
    public async Task GetConversionAsync_WithProvider_IncludesProviderInQuery()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse(ValidJsonResponse);
        var client = testBuilder.Build();

        await client.GetConversionAsync(new ConversionRequest
        {
            BaseCurrency = "EUR",
            ToCurrency = "USD",
            Amount = 100,
            Provider = "frankfurter"
        }, TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("Provider=frankfurter");
    }

    [Fact]
    public async Task GetConversionAsync_ErrorResponse_ThrowsCurrencyConverterApiException()
    {
        var testBuilder = new TestBuilder()
            .WithErrorResponse(HttpStatusCode.InternalServerError, "server error");
        var client = testBuilder.Build();

        var act = () => client.GetConversionAsync(
            new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 }, TestContext.Current.CancellationToken);

        await act.Should().ThrowExactlyAsync<CurrencyConverterApiException>();
    }

    [Fact]
    public async Task GetConversionAsync_NullJsonPayload_ThrowsCurrencyConverterApiException()
    {
        var testBuilder = new TestBuilder().WithSuccessResponse("null");
        var client = testBuilder.Build();

        var act = () => client.GetConversionAsync(
            new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 }, TestContext.Current.CancellationToken);

        await act.Should().ThrowExactlyAsync<CurrencyConverterApiException>();
    }

    [Fact]
    public void DateOnlyJsonConverter_Write_SerializesDateAsYearMonthDay()
    {
        var jsonOptions = GetJsonOptions();
        var date = new DateOnly(2024, 3, 15);

        var json = JsonSerializer.Serialize(date, jsonOptions);

        json.Should().Be("\"2024-03-15\"");
    }

    [Fact]
    public void DateOnlyJsonConverter_WriteAsPropertyName_SerializesDateKeyAsYearMonthDay()
    {
        var jsonOptions = GetJsonOptions();
        var dict = new Dictionary<DateOnly, string> { [new DateOnly(2024, 3, 15)] = "value" };

        var json = JsonSerializer.Serialize(dict, jsonOptions);

        json.Should().Contain("2024-03-15");
    }

    private static JsonSerializerOptions GetJsonOptions()
    {
        var field = typeof(CurrencyConverterClient)
            .GetField("JsonOptions", BindingFlags.NonPublic | BindingFlags.Static);
        return (JsonSerializerOptions)field!.GetValue(null)!;
    }
}
