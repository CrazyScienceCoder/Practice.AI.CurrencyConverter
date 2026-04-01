using System.Net;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Tests.Plugins;

public sealed partial class CurrencyConverterPluginSpecifications
{
    [Fact]
    public async Task GetLatestExchangeRatesAsync_ClientSucceeds_ReturnsSuccess()
    {
        var response = TestBuilder.BuildExchangeRateResponse();
        var sut = new TestBuilder().WithLatestResult(response).Build();

        var result = await sut.GetLatestExchangeRatesAsync("EUR", TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_ClientSucceeds_DataMatchesClientResponse()
    {
        var response = TestBuilder.BuildExchangeRateResponse();
        var sut = new TestBuilder().WithLatestResult(response).Build();

        var result = await sut.GetLatestExchangeRatesAsync("EUR", TestContext.Current.CancellationToken);

        result.Data.Should().BeSameAs(response);
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_ClientThrowsUnauthorized_ReturnsAuthFailure()
    {
        var ex = TestBuilder.CreateApiException(HttpStatusCode.Unauthorized);
        var sut = new TestBuilder().WithLatestException(ex).Build();

        var result = await sut.GetLatestExchangeRatesAsync("EUR", TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not authorized");
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_ClientThrowsForbidden_ReturnsAuthFailure()
    {
        var ex = TestBuilder.CreateApiException(HttpStatusCode.Forbidden);
        var sut = new TestBuilder().WithLatestException(ex).Build();

        var result = await sut.GetLatestExchangeRatesAsync("EUR", TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not authorized");
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_ClientThrowsGenericException_ReturnsFailureWithMessage()
    {
        var sut = new TestBuilder()
            .WithLatestException(new InvalidOperationException("network error"))
            .Build();

        var result = await sut.GetLatestExchangeRatesAsync("EUR", TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("EUR");
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_ClientThrowsNonAuthApiException_ReturnsGenericFailure()
    {
        var ex = TestBuilder.CreateApiException(HttpStatusCode.InternalServerError);
        var sut = new TestBuilder().WithLatestException(ex).Build();

        var result = await sut.GetLatestExchangeRatesAsync("EUR", TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public async Task ConvertCurrencyAsync_ClientSucceeds_ReturnsSuccess()
    {
        var response = TestBuilder.BuildExchangeRateResponse();
        var sut = new TestBuilder().WithConversionResult(response).Build();

        var result = await sut.ConvertCurrencyAsync("USD", "EUR", 100m, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ConvertCurrencyAsync_ClientSucceeds_DataMatchesClientResponse()
    {
        var response = TestBuilder.BuildExchangeRateResponse();
        var sut = new TestBuilder().WithConversionResult(response).Build();

        var result = await sut.ConvertCurrencyAsync("USD", "EUR", 100m, TestContext.Current.CancellationToken);

        result.Data.Should().BeSameAs(response);
    }

    [Fact]
    public async Task ConvertCurrencyAsync_ClientThrowsUnauthorized_ReturnsAuthFailure()
    {
        var ex = TestBuilder.CreateApiException(HttpStatusCode.Unauthorized);
        var sut = new TestBuilder().WithConversionException(ex).Build();

        var result = await sut.ConvertCurrencyAsync("USD", "EUR", 100m, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not authorized");
    }

    [Fact]
    public async Task ConvertCurrencyAsync_ClientThrowsForbidden_ReturnsAuthFailure()
    {
        var ex = TestBuilder.CreateApiException(HttpStatusCode.Forbidden);
        var sut = new TestBuilder().WithConversionException(ex).Build();

        var result = await sut.ConvertCurrencyAsync("USD", "EUR", 100m, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not authorized");
    }

    [Fact]
    public async Task ConvertCurrencyAsync_ClientThrowsGenericException_ReturnsFailureWithCurrencyInfo()
    {
        var sut = new TestBuilder()
            .WithConversionException(new InvalidOperationException("timeout"))
            .Build();

        var result = await sut.ConvertCurrencyAsync("USD", "EUR", 100m, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("USD").And.Contain("EUR");
    }

    [Fact]
    public async Task ConvertCurrencyAsync_ClientThrowsNonAuthApiException_ReturnsGenericFailure()
    {
        var ex = TestBuilder.CreateApiException(HttpStatusCode.BadGateway);
        var sut = new TestBuilder().WithConversionException(ex).Build();

        var result = await sut.ConvertCurrencyAsync("USD", "EUR", 100m, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_ClientSucceeds_ReturnsSuccess()
    {
        var response = TestBuilder.BuildHistoricalResponse();
        var sut = new TestBuilder().WithHistoricalResult(response).Build();

        var result = await sut.GetHistoricalExchangeRatesAsync(
            "EUR", "2024-01-01", "2024-01-15", TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_ClientSucceeds_DataMatchesClientResponse()
    {
        var response = TestBuilder.BuildHistoricalResponse();
        var sut = new TestBuilder().WithHistoricalResult(response).Build();

        var result = await sut.GetHistoricalExchangeRatesAsync(
            "EUR", "2024-01-01", "2024-01-15", TestContext.Current.CancellationToken);

        result.Data.Should().BeSameAs(response);
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_ClientThrowsUnauthorized_ReturnsAuthFailure()
    {
        var ex = TestBuilder.CreateApiException(HttpStatusCode.Unauthorized);
        var sut = new TestBuilder().WithHistoricalException(ex).Build();

        var result = await sut.GetHistoricalExchangeRatesAsync(
            "EUR", "2024-01-01", "2024-01-15", TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not authorized");
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_ClientThrowsForbidden_ReturnsAuthFailure()
    {
        var ex = TestBuilder.CreateApiException(HttpStatusCode.Forbidden);
        var sut = new TestBuilder().WithHistoricalException(ex).Build();

        var result = await sut.GetHistoricalExchangeRatesAsync(
            "EUR", "2024-01-01", "2024-01-15", TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not authorized");
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_ClientThrowsGenericException_ReturnsGenericFailure()
    {
        var sut = new TestBuilder()
            .WithHistoricalException(new HttpRequestException("connection refused"))
            .Build();

        var result = await sut.GetHistoricalExchangeRatesAsync(
            "EUR", "2024-01-01", "2024-01-15", TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_ClientThrowsNonAuthApiException_ReturnsGenericFailure()
    {
        var ex = TestBuilder.CreateApiException(HttpStatusCode.ServiceUnavailable);
        var sut = new TestBuilder().WithHistoricalException(ex).Build();

        var result = await sut.GetHistoricalExchangeRatesAsync(
            "EUR", "2024-01-01", "2024-01-15", TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }
}
