using System.Net;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Exceptions;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Tests.Clients;

public partial class FrankfurterApiClientSpecifications
{
    [Fact]
    public async Task GetLatestAsync_SuccessfulResponse_ReturnsLatestResponse()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":1,"base":"EUR","date":"2024-01-15","rates":{"USD":1.08}}""");

        var client = testBuilder.Build();

        var result = await client.GetLatestAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.Base.Should().Be("EUR");
        result.Amount.Should().Be(1);
    }

    [Fact]
    public async Task GetLatestAsync_SuccessfulResponse_ReturnsRates()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":1,"base":"EUR","date":"2024-01-15","rates":{"USD":1.08,"GBP":0.86}}""");

        var client = testBuilder.Build();

        var result = await client.GetLatestAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Rates.Should().ContainKey("USD");
        result.Rates.Should().ContainKey("GBP");
    }

    [Fact]
    public async Task GetLatestAsync_WithCustomBaseCurrency_SendsRequestWithBaseQueryParam()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":1,"base":"USD","date":"2024-01-15","rates":{"EUR":0.92}}""");

        var client = testBuilder.Build();

        await client.GetLatestAsync(baseCurrency: "USD", cancellationToken: TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("base=USD");
    }

    [Fact]
    public async Task GetLatestAsync_WithAmount_SendsRequestWithAmountQueryParam()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":100,"base":"EUR","date":"2024-01-15","rates":{"USD":108}}""");

        var client = testBuilder.Build();

        await client.GetLatestAsync(amount: 100, cancellationToken: TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("amount=100");
    }

    [Fact]
    public async Task GetLatestAsync_WithSymbols_SendsRequestWithSymbolsQueryParam()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":1,"base":"EUR","date":"2024-01-15","rates":{"USD":1.08}}""");

        var client = testBuilder.Build();

        await client.GetLatestAsync(symbols: ["USD", "GBP"], cancellationToken: TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("symbols=");
        testBuilder.LastRequestPathAndQuery.Should().Contain("USD");
        testBuilder.LastRequestPathAndQuery.Should().Contain("GBP");
    }

    [Fact]
    public async Task GetLatestAsync_Always_SendsRequestToLatestEndpoint()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":1,"base":"EUR","date":"2024-01-15","rates":{"USD":1.08}}""");

        var client = testBuilder.Build();

        await client.GetLatestAsync(cancellationToken: TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("latest");
    }

    [Fact]
    public async Task GetLatestAsync_ErrorResponse_ThrowsFrankfurterApiException()
    {
        var testBuilder = new TestBuilder()
            .WithErrorResponse(HttpStatusCode.NotFound, "not found");

        var client = testBuilder.Build();

        var act = () => client.GetLatestAsync(cancellationToken: TestContext.Current.CancellationToken);

        await act.Should().ThrowExactlyAsync<FrankfurterApiException>();
    }

    [Fact]
    public async Task GetLatestAsync_ErrorResponse_ExceptionContainsStatusCode()
    {
        var testBuilder = new TestBuilder()
            .WithErrorResponse(HttpStatusCode.ServiceUnavailable, "service unavailable");

        var client = testBuilder.Build();

        var act = () => client.GetLatestAsync(cancellationToken: TestContext.Current.CancellationToken);

        (await act.Should().ThrowExactlyAsync<FrankfurterApiException>())
            .Which.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task GetLatestAsync_NullJsonPayload_ThrowsFrankfurterApiException()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("null");

        var client = testBuilder.Build();

        var act = () => client.GetLatestAsync(cancellationToken: TestContext.Current.CancellationToken);

        (await act.Should().ThrowExactlyAsync<FrankfurterApiException>())
            .Which.Message.Should().Contain("empty payload");
    }

    [Fact]
    public async Task GetHistoricalAsync_SuccessfulResponse_ReturnsHistoricalResponse()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":1,"base":"EUR","date":"2024-01-15","rates":{"USD":1.08}}""");

        var client = testBuilder.Build();

        var result = await client.GetHistoricalAsync(new DateOnly(2024, 1, 15), cancellationToken: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.Base.Should().Be("EUR");
    }

    [Fact]
    public async Task GetHistoricalAsync_WithDate_SendsRequestWithDateInPath()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":1,"base":"EUR","date":"2024-01-15","rates":{"USD":1.08}}""");

        var client = testBuilder.Build();

        await client.GetHistoricalAsync(new DateOnly(2024, 1, 15), cancellationToken: TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("2024-01-15");
    }

    [Fact]
    public async Task GetHistoricalAsync_WithCustomBaseCurrency_SendsRequestWithBaseQueryParam()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":1,"base":"USD","date":"2024-01-15","rates":{"EUR":0.92}}""");

        var client = testBuilder.Build();

        await client.GetHistoricalAsync(new DateOnly(2024, 1, 15), baseCurrency: "USD", cancellationToken: TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("base=USD");
    }

    [Fact]
    public async Task GetHistoricalAsync_ErrorResponse_ThrowsFrankfurterApiException()
    {
        var testBuilder = new TestBuilder()
            .WithErrorResponse(HttpStatusCode.BadRequest, "bad request");

        var client = testBuilder.Build();

        var act = () => client.GetHistoricalAsync(new DateOnly(2024, 1, 15), cancellationToken: TestContext.Current.CancellationToken);

        await act.Should().ThrowExactlyAsync<FrankfurterApiException>();
    }

    [Fact]
    public async Task GetHistoricalAsync_NullJsonPayload_ThrowsFrankfurterApiException()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("null");

        var client = testBuilder.Build();

        var act = () => client.GetHistoricalAsync(new DateOnly(2024, 1, 15), cancellationToken: TestContext.Current.CancellationToken);

        (await act.Should().ThrowExactlyAsync<FrankfurterApiException>())
            .Which.Message.Should().Contain("empty payload");
    }

    [Fact]
    public async Task GetTimeSeriesAsync_SuccessfulResponse_ReturnsTimeSeriesResponse()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":1,"base":"EUR","start_date":"2024-01-01","end_date":"2024-01-31","rates":{"2024-01-15":{"USD":1.08}}}""");

        var client = testBuilder.Build();

        var result = await client.GetTimeSeriesAsync(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31), cancellationToken: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.Base.Should().Be("EUR");
    }

    [Fact]
    public async Task GetTimeSeriesAsync_SuccessfulResponse_ReturnsRatesByDate()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":1,"base":"EUR","start_date":"2024-01-01","end_date":"2024-01-31","rates":{"2024-01-15":{"USD":1.08}}}""");

        var client = testBuilder.Build();

        var result = await client.GetTimeSeriesAsync(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31), cancellationToken: TestContext.Current.CancellationToken);

        result.Rates.Should().ContainKey("2024-01-15");
    }

    [Fact]
    public async Task GetTimeSeriesAsync_WithDateRange_SendsRequestWithRangeInPath()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":1,"base":"EUR","start_date":"2024-01-01","end_date":"2024-01-31","rates":{"2024-01-15":{"USD":1.08}}}""");

        var client = testBuilder.Build();

        await client.GetTimeSeriesAsync(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31), cancellationToken: TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("2024-01-01");
        testBuilder.LastRequestPathAndQuery.Should().Contain("2024-01-31");
    }

    [Fact]
    public async Task GetTimeSeriesAsync_WithCustomBaseCurrency_SendsRequestWithBaseQueryParam()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":1,"base":"USD","start_date":"2024-01-01","end_date":"2024-01-31","rates":{"2024-01-15":{"EUR":0.92}}}""");

        var client = testBuilder.Build();

        await client.GetTimeSeriesAsync(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31), baseCurrency: "USD", cancellationToken: TestContext.Current.CancellationToken);

        testBuilder.LastRequestPathAndQuery.Should().Contain("base=USD");
    }

    [Fact]
    public async Task GetTimeSeriesAsync_ErrorResponse_ThrowsFrankfurterApiException()
    {
        var testBuilder = new TestBuilder()
            .WithErrorResponse(HttpStatusCode.InternalServerError, "server error");

        var client = testBuilder.Build();

        var act = () => client.GetTimeSeriesAsync(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31), cancellationToken: TestContext.Current.CancellationToken);

        await act.Should().ThrowExactlyAsync<FrankfurterApiException>();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_NullJsonPayload_ThrowsFrankfurterApiException()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("null");

        var client = testBuilder.Build();

        var act = () => client.GetTimeSeriesAsync(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31), cancellationToken: TestContext.Current.CancellationToken);

        (await act.Should().ThrowExactlyAsync<FrankfurterApiException>())
            .Which.Message.Should().Contain("empty payload");
    }

    [Fact]
    public async Task GetLatestAsync_SuccessfulResponse_ReturnsDate()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":1,"base":"EUR","date":"2024-01-15","rates":{"USD":1.08}}""");

        var client = testBuilder.Build();

        var result = await client.GetLatestAsync(cancellationToken: TestContext.Current.CancellationToken);

        result.Date.Should().Be(new DateTime(2024, 1, 15));
    }

    [Fact]
    public async Task GetTimeSeriesAsync_SuccessfulResponse_ReturnsAmount()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":100,"base":"EUR","start_date":"2024-01-01","end_date":"2024-01-31","rates":{"2024-01-15":{"USD":1.08}}}""");

        var client = testBuilder.Build();

        var result = await client.GetTimeSeriesAsync(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31), cancellationToken: TestContext.Current.CancellationToken);

        result.Amount.Should().Be(100);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_SuccessfulResponse_ReturnsStartDate()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":1,"base":"EUR","start_date":"2024-01-01","end_date":"2024-01-31","rates":{"2024-01-15":{"USD":1.08}}}""");

        var client = testBuilder.Build();

        var result = await client.GetTimeSeriesAsync(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31), cancellationToken: TestContext.Current.CancellationToken);

        result.StartDate.Should().Be(new DateTime(2024, 1, 1));
    }

    [Fact]
    public async Task GetTimeSeriesAsync_SuccessfulResponse_ReturnsEndDate()
    {
        var testBuilder = new TestBuilder()
            .WithSuccessResponse("""{"amount":1,"base":"EUR","start_date":"2024-01-01","end_date":"2024-01-31","rates":{"2024-01-15":{"USD":1.08}}}""");

        var client = testBuilder.Build();

        var result = await client.GetTimeSeriesAsync(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 31), cancellationToken: TestContext.Current.CancellationToken);

        result.EndDate.Should().Be(new DateTime(2024, 1, 31));
    }
}
