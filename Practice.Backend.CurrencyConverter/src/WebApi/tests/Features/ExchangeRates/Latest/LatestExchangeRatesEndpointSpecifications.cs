using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Features.ExchangeRates.Latest;

public partial class LatestExchangeRatesEndpointSpecifications
{
    [Fact]
    public async Task InvokeAsync_MediatorReturnsSuccess_ReturnsOkResult()
    {
        var testBuilder = new TestBuilder()
            .SetupMediatorSuccess();

        var endpoint = testBuilder.Build();

        var result = await endpoint.InvokeAsync(testBuilder.DefaultRequest, TestContext.Current.CancellationToken);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task InvokeAsync_MediatorReturnsSuccess_MediatorCalledWithMappedQuery()
    {
        var testBuilder = new TestBuilder()
            .SetupMediatorSuccess();

        var endpoint = testBuilder.Build();

        await endpoint.InvokeAsync(testBuilder.DefaultRequest, TestContext.Current.CancellationToken);

        testBuilder.MediatorMock.Verify(
            m => m.Send(
                It.Is<GetLatestExchangeRateQuery>(q =>
                    q.BaseCurrency == testBuilder.DefaultRequest.BaseCurrency),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_MediatorReturnsNotFound_ReturnsNotFoundResult()
    {
        var testBuilder = new TestBuilder()
            .SetupMediatorNotFound();

        var endpoint = testBuilder.Build();

        var result = await endpoint.InvokeAsync(testBuilder.DefaultRequest, TestContext.Current.CancellationToken);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task InvokeAsync_MediatorReturnsFailure_Returns500Result()
    {
        var testBuilder = new TestBuilder()
            .SetupMediatorGenericFailure();

        var endpoint = testBuilder.Build();

        var result = await endpoint.InvokeAsync(testBuilder.DefaultRequest, TestContext.Current.CancellationToken);

        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task InvokeAsync_RequestWithNullProvider_DefaultsToFrankfurterProvider()
    {
        var testBuilder = new TestBuilder()
            .SetupMediatorSuccess();

        var endpoint = testBuilder.Build();

        var request = new LatestExchangeRatesRequest { BaseCurrency = "USD", Provider = null };
        await endpoint.InvokeAsync(request, TestContext.Current.CancellationToken);

        testBuilder.MediatorMock.Verify(
            m => m.Send(
                It.Is<GetLatestExchangeRateQuery>(q =>
                    q.Provider == Domain.Types.ExchangeRateProvider.Frankfurter),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
