using System.Net;
using ErrorOr;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Exceptions;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Models;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.ExchangeRateProviders.Frankfurter;

public sealed partial class ExchangeRateSnapshotProviderSpecifications
{
    private static readonly Currency BaseCurrency = Currency.Create("EUR");
    private static readonly ExchangeDate From = ExchangeDate.Create(new DateOnly(2024, 1, 1));
    private static readonly ExchangeDate To = ExchangeDate.Create(new DateOnly(2024, 1, 15));

    private static LatestResponse BuildLatestResponse() => new()
    {
        Amount = 1,
        Base = "EUR",
        Date = new DateTime(2024, 1, 15),
        Rates = new() { { "USD", 1.08 }, { "GBP", 0.86 } }
    };

    private static TimeSeriesResponse BuildTimeSeriesResponse() => new()
    {
        Amount = 1,
        Base = "EUR",
        StartDate = new DateTime(2024, 1, 1),
        EndDate = new DateTime(2024, 1, 15),
        Rates = new()
        {
            { "2024-01-01", new() { { "USD", 1.08 } } },
            { "2024-01-02", new() { { "USD", 1.09 } } }
        }
    };

    [Fact]
    public void Provider_Always_ReturnsFrankfurter()
    {
        var sut = new TestBuilder().Build();

        sut.Provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }

    [Fact]
    public async Task GetLatestAsync_SuccessfulApiResponse_ReturnsSnapshot()
    {
        var response = BuildLatestResponse();
        var sut = new TestBuilder().WithLatestResponse(response).Build();

        var result = await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task GetLatestAsync_SuccessfulApiResponse_SnapshotHasCorrectBase()
    {
        var response = BuildLatestResponse();
        var sut = new TestBuilder().WithLatestResponse(response).Build();

        var result = await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        result.Value.Base.Value.Should().Be(response.Base);
    }

    [Fact]
    public async Task GetLatestAsync_SuccessfulApiResponse_SnapshotHasCorrectRates()
    {
        var response = BuildLatestResponse();
        var sut = new TestBuilder().WithLatestResponse(response).Build();

        var result = await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        result.Value.Rates.Should().HaveCount(response.Rates.Count);
        result.Value.Rates.Should().ContainKey(Currency.Create("USD"));
    }

    [Fact]
    public async Task GetLatestAsync_SuccessfulApiResponse_SnapshotHasCorrectDate()
    {
        var response = BuildLatestResponse();
        var sut = new TestBuilder().WithLatestResponse(response).Build();

        var result = await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        result.Value.Date.Value.Should().Be(DateOnly.FromDateTime(response.Date));
    }

    [Fact]
    public async Task GetLatestAsync_ApiReturnsNotFound_ReturnsNotFoundError()
    {
        var exception = new FrankfurterApiException("Not found", HttpStatusCode.NotFound);
        var sut = new TestBuilder().WithLatestThrows(exception).Build();

        var result = await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetLatestAsync_ApiThrowsUnexpectedException_ReturnsUnexpectedError()
    {
        var exception = new InvalidOperationException("Something went wrong");
        var sut = new TestBuilder().WithLatestThrows(exception).Build();

        var result = await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Unexpected);
    }

    [Fact]
    public async Task GetLatestAsync_ApiThrowsUnexpectedException_ErrorDescriptionContainsMessage()
    {
        var exceptionMessage = "Something went wrong";
        var exception = new InvalidOperationException(exceptionMessage);
        var sut = new TestBuilder().WithLatestThrows(exception).Build();

        var result = await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        result.FirstError.Description.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task GetLatestAsync_ApiThrowsFrankfurterExceptionWithNonNotFoundStatus_ReturnsUnexpectedError()
    {
        var exception = new FrankfurterApiException("Server error", HttpStatusCode.InternalServerError);
        var sut = new TestBuilder().WithLatestThrows(exception).Build();

        var result = await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Unexpected);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_SuccessfulApiResponse_ReturnsHistoricalSnapshot()
    {
        var response = BuildTimeSeriesResponse();
        var sut = new TestBuilder().WithTimeSeriesResponse(response).Build();

        var result = await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_SuccessfulApiResponse_SnapshotHasCorrectBase()
    {
        var response = BuildTimeSeriesResponse();
        var sut = new TestBuilder().WithTimeSeriesResponse(response).Build();

        var result = await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.Value.Base.Value.Should().Be(response.Base);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_SuccessfulApiResponse_SnapshotHasCorrectDateRange()
    {
        var response = BuildTimeSeriesResponse();
        var sut = new TestBuilder().WithTimeSeriesResponse(response).Build();

        var result = await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.Value.StartDate.Value.Should().Be(DateOnly.FromDateTime(response.StartDate));
        result.Value.EndDate.Value.Should().Be(DateOnly.FromDateTime(response.EndDate));
    }

    [Fact]
    public async Task GetTimeSeriesAsync_SuccessfulApiResponse_SnapshotHasCorrectRates()
    {
        var response = BuildTimeSeriesResponse();
        var sut = new TestBuilder().WithTimeSeriesResponse(response).Build();

        var result = await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.Value.Rates.Should().HaveCount(response.Rates.Count);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_ApiReturnsNotFound_ReturnsNotFoundError()
    {
        var exception = new FrankfurterApiException("Not found", HttpStatusCode.NotFound);
        var sut = new TestBuilder().WithTimeSeriesThrows(exception).Build();

        var result = await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_ApiThrowsUnexpectedException_ReturnsUnexpectedError()
    {
        var exception = new InvalidOperationException("Something went wrong");
        var sut = new TestBuilder().WithTimeSeriesThrows(exception).Build();

        var result = await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Unexpected);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_ApiThrowsUnexpectedException_ErrorDescriptionContainsMessage()
    {
        var exceptionMessage = "Something went wrong";
        var exception = new InvalidOperationException(exceptionMessage);
        var sut = new TestBuilder().WithTimeSeriesThrows(exception).Build();

        var result = await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.FirstError.Description.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_ApiThrowsFrankfurterExceptionWithNonNotFoundStatus_ReturnsUnexpectedError()
    {
        var exception = new FrankfurterApiException("Server error", HttpStatusCode.InternalServerError);
        var sut = new TestBuilder().WithTimeSeriesThrows(exception).Build();

        var result = await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Unexpected);
    }
}
