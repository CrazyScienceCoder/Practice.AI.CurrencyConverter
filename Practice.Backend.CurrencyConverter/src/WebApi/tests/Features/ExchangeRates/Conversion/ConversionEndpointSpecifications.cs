using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.CurrencyConversion;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Features.ExchangeRates.Conversion;

public partial class ConversionEndpointSpecifications
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
                It.Is<GetCurrencyConversionQuery>(q =>
                    q.BaseCurrency == testBuilder.DefaultRequest.BaseCurrency &&
                    q.ToCurrency == testBuilder.DefaultRequest.ToCurrency &&
                    q.Amount == testBuilder.DefaultRequest.Amount),
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
    public async Task InvokeAsync_MediatorReturnsNotAllowed_ReturnsBadRequestResult()
    {
        var testBuilder = new TestBuilder()
            .SetupMediatorNotAllowed();

        var endpoint = testBuilder.Build();

        var result = await endpoint.InvokeAsync(testBuilder.DefaultRequest, TestContext.Current.CancellationToken);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task InvokeAsync_RequestWithProvider_MediatorCalledWithCorrectProvider()
    {
        var testBuilder = new TestBuilder()
            .SetupMediatorSuccess();

        var endpoint = testBuilder.Build();

        var request = new ConversionRequest
        {
            BaseCurrency = "USD",
            ToCurrency = "EUR",
            Amount = 100m,
            Provider = "Frankfurter"
        };

        await endpoint.InvokeAsync(request, TestContext.Current.CancellationToken);

        testBuilder.MediatorMock.Verify(
            m => m.Send(
                It.Is<GetCurrencyConversionQuery>(q =>
                    q.Provider == Domain.Types.ExchangeRateProvider.Frankfurter),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
