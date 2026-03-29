using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetHistorical;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Features.ExchangeRates.Historical;

public partial class HistoricalExchangeRateEndpointSpecifications
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
                It.Is<GetHistoricalExchangeRateQuery>(q =>
                    q.BaseCurrency == testBuilder.DefaultRequest.BaseCurrency &&
                    q.From == testBuilder.DefaultRequest.From!.Value &&
                    q.To == testBuilder.DefaultRequest.To!.Value),
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
    public async Task InvokeAsync_MediatorReturnsGenericFailure_Returns500Result()
    {
        var testBuilder = new TestBuilder()
            .SetupMediatorGenericFailure();

        var endpoint = testBuilder.Build();

        var result = await endpoint.InvokeAsync(testBuilder.DefaultRequest, TestContext.Current.CancellationToken);

        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task InvokeAsync_RequestWithPaginationParams_MediatorCalledWithPaginationQuery()
    {
        var testBuilder = new TestBuilder()
            .SetupMediatorSuccess();

        var endpoint = testBuilder.Build();

        var request = new HistoricalExchangeRateRequest
        {
            BaseCurrency = "USD",
            From = new DateOnly(2025, 1, 1),
            To = new DateOnly(2025, 1, 10),
            PageNumber = 2,
            DaysPerPage = 5
        };

        await endpoint.InvokeAsync(request, TestContext.Current.CancellationToken);

        testBuilder.MediatorMock.Verify(
            m => m.Send(
                It.Is<GetHistoricalExchangeRateQuery>(q =>
                    q.PageNumber == 2 &&
                    q.DaysPerPage == 5),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
