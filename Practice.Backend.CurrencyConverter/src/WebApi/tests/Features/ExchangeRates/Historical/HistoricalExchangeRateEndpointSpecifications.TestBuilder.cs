using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetHistorical;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Builders;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Factories;
using Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Historical;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Features.ExchangeRates.Historical;

public partial class HistoricalExchangeRateEndpointSpecifications
{
    private class TestBuilder
    {
        public readonly Mock<IMediator> MediatorMock = new();
        private readonly Mock<IActionResultBuilderFactory> _factoryMock = new();
        private readonly Mock<IActionResultBuilder> _builderMock = new();

        public readonly HistoricalExchangeRateRequest DefaultRequest = new()
        {
            BaseCurrency = "USD",
            From = new DateOnly(2025, 1, 1),
            To = new DateOnly(2025, 1, 10),
            Provider = null
        };

        public TestBuilder()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/api/v1/exchange-rate/historical";
        }

        public TestBuilder SetupMediatorSuccess()
        {
            var successResponse = GetHistoricalExchangeRateQueryResponse.Success(
                new GetHistoricalExchangeRateQueryResult
                {
                    Amount = 1m,
                    Base = "USD",
                    StartDate = new DateOnly(2025, 1, 1),
                    EndDate = new DateOnly(2025, 1, 10),
                    PageNumber = 1,
                    HasMore = false,
                    TotalNumberOfPages = 1
                });

            MediatorMock
                .Setup(m => m.Send(It.IsAny<GetHistoricalExchangeRateQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(successResponse);

            _builderMock
                .Setup(b => b.Build(It.IsAny<GetHistoricalExchangeRateQueryResponse>()))
                .Returns(new OkObjectResult(successResponse.Data));

            _factoryMock
                .Setup(f => f.Create(It.IsAny<GetHistoricalExchangeRateQueryResponse>()))
                .Returns(_builderMock.Object);

            return this;
        }

        public TestBuilder SetupMediatorNotFound()
        {
            var notFoundResponse = GetHistoricalExchangeRateQueryResponse.Failure(errorType: ErrorType.NotFound);

            MediatorMock
                .Setup(m => m.Send(It.IsAny<GetHistoricalExchangeRateQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(notFoundResponse);

            _builderMock
                .Setup(b => b.Build(It.IsAny<GetHistoricalExchangeRateQueryResponse>()))
                .Returns(new NotFoundObjectResult(new ProblemDetails()));

            _factoryMock
                .Setup(f => f.Create(It.IsAny<GetHistoricalExchangeRateQueryResponse>()))
                .Returns(_builderMock.Object);

            return this;
        }

        public TestBuilder SetupMediatorGenericFailure()
        {
            var failureResponse = GetHistoricalExchangeRateQueryResponse.Failure(errorType: ErrorType.Generic);

            MediatorMock
                .Setup(m => m.Send(It.IsAny<GetHistoricalExchangeRateQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(failureResponse);

            _builderMock
                .Setup(b => b.Build(It.IsAny<GetHistoricalExchangeRateQueryResponse>()))
                .Returns(new ObjectResult(new ProblemDetails()) { StatusCode = StatusCodes.Status500InternalServerError });

            _factoryMock
                .Setup(f => f.Create(It.IsAny<GetHistoricalExchangeRateQueryResponse>()))
                .Returns(_builderMock.Object);

            return this;
        }

        public HistoricalExchangeRateEndpoint Build()
            => new(MediatorMock.Object, _factoryMock.Object);
    }
}
