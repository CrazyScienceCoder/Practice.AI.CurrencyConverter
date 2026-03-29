using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Practice.Backend.CurrencyConverter.Integration.Tests.Infrastructure;

namespace Practice.Backend.CurrencyConverter.Integration.Tests.ExchangeRates.Historical;

public partial class HistoricalExchangeRateIntegrationSpecifications
{
    [Fact]
    public async Task InvokeAsync_WithValidAuthenticatedRequest_Returns200()
    {
        const string currency = "EUR";
        SetupSuccessResponse(currency);
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency={currency}&from={FromDate}&to={ToDate}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonNode.Parse(body)!;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        json["base"]!.GetValue<string>().Should().Be(currency);
        json["amount"]!.GetValue<decimal>().Should().Be(1m);
        json["startDate"]!.GetValue<string>().Should().Be(FromDate);
        json["endDate"]!.GetValue<string>().Should().Be(ToDate);
        json["pageNumber"]!.GetValue<int>().Should().Be(1);
        json["hasMore"]!.GetValue<bool>().Should().BeFalse();
        json["totalNumberOfPages"]!.GetValue<int>().Should().Be(1);
        json["rates"]!["2025-01-06"]!["USD"]!.GetValue<decimal>().Should().Be(1.09m);
    }

    [Fact]
    public async Task InvokeAsync_WithValidAuthenticatedRequest_ResponseBodyContainsRates()
    {
        const string currency = "CHF";
        SetupSuccessResponse(currency);
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency={currency}&from={FromDate}&to={ToDate}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonNode.Parse(body)!;

        json["base"]!.GetValue<string>().Should().Be(currency);
        json["amount"]!.GetValue<decimal>().Should().Be(1m);
        json["rates"]!["2025-01-06"]!["USD"]!.GetValue<decimal>().Should().Be(1.09m);
        json["rates"]!["2025-01-07"]!["USD"]!.GetValue<decimal>().Should().Be(1.10m);
        json["rates"]!["2025-01-08"]!["USD"]!.GetValue<decimal>().Should().Be(1.08m);
        json["rates"]!["2025-01-09"]!["USD"]!.GetValue<decimal>().Should().Be(1.11m);
        json["rates"]!["2025-01-10"]!["USD"]!.GetValue<decimal>().Should().Be(1.07m);
    }

    [Fact]
    public async Task InvokeAsync_WithoutAuthorizationHeader_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency=EUR&from={FromDate}&to={ToDate}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidBearerToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "not-a-valid-jwt");

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency=EUR&from={FromDate}&to={ToDate}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_WithTokenMissingRequiredRole_Returns403()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken("wrong:role"));

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency=EUR&from={FromDate}&to={ToDate}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task InvokeAsync_WithForbiddenCurrencyMXN_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency=MXN&from={FromDate}&to={ToDate}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithForbiddenCurrencyPLN_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency=PLN&from={FromDate}&to={ToDate}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidCurrencyCode_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency=INVALID&from={FromDate}&to={ToDate}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithNullFromDate_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency=EUR&to={ToDate}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithNullToDate_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency=EUR&from={FromDate}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithFutureFromDate_Returns400()
    {
        AuthorizeClient();
        var futureDate = DateTime.UtcNow.AddDays(10).ToString("yyyy-MM-dd");

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency=EUR&from={futureDate}&to={ToDate}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithFutureToDate_Returns400()
    {
        AuthorizeClient();
        var futureDate = DateTime.UtcNow.AddDays(10).ToString("yyyy-MM-dd");

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency=EUR&from={FromDate}&to={futureDate}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithPageNumberLessThanOne_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency=EUR&from={FromDate}&to={ToDate}&pageNumber=0",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithDaysPerPageExceedingMaximum_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency=EUR&from={FromDate}&to={ToDate}&daysPerPage=31",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithValidPaginationParameters_Returns200()
    {
        const string currency = "DKK";
        SetupSuccessResponse(currency);
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency={currency}&from={FromDate}&to={ToDate}&pageNumber=1&daysPerPage=5",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonNode.Parse(body)!;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        json["base"]!.GetValue<string>().Should().Be(currency);
        json["pageNumber"]!.GetValue<int>().Should().Be(1);
        json["hasMore"]!.GetValue<bool>().Should().BeFalse();
        json["totalNumberOfPages"]!.GetValue<int>().Should().Be(1);
        json["rates"]!.AsObject().Count.Should().Be(5);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidProvider_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency=EUR&from={FromDate}&to={ToDate}&provider=UnknownProvider",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WhenProviderReturnsNotFound_Returns404()
    {
        const string currency = "GBP";
        SetupNotFoundResponse(currency);
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{HistoricalUrl}?baseCurrency={currency}&from={FromDate}&to={ToDate}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
