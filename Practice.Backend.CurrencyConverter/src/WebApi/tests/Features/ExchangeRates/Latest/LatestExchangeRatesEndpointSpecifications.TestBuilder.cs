using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Builders;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Factories;
using Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Latest;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Features.ExchangeRates.Latest;

public partial class LatestExchangeRatesEndpointSpecifications
{
    private class TestBuilder
    {
        public readonly Mock<IMediator> MediatorMock = new();
        private readonly Mock<IActionResultBuilderFactory> _factoryMock = new();
        private readonly Mock<IActionResultBuilder> _builderMock = new();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
        private readonly Mock<ProblemDetailsFactory> _problemDetailsFactoryMock = new();

        public readonly LatestExchangeRatesRequest DefaultRequest = new()
        {
            BaseCurrency = "USD",
            Provider = null
        };

        public TestBuilder()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/api/v1/exchange-rate/latest";

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            _problemDetailsFactoryMock
                .Setup(x => x.CreateProblemDetails(
                    It.IsAny<HttpContext>(), It.IsAny<int?>(), It.IsAny<string?>(),
                    It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
                .Returns((HttpContext ctx, int? status, string? title, string? type, string? detail, string? instance) =>
                    new ProblemDetails { Status = status, Title = title, Detail = detail });

            _problemDetailsFactoryMock
                .Setup(x => x.CreateValidationProblemDetails(
                    It.IsAny<HttpContext>(), It.IsAny<ModelStateDictionary>(), It.IsAny<int?>(),
                    It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
                .Returns(new ValidationProblemDetails());
        }

        public TestBuilder SetupMediatorSuccess()
        {
            var successResponse = GetLatestExchangeRateQueryResponse.Success(
                new GetLatestExchangeRateQueryResult
                {
                    Amount = 1m,
                    Base = "USD",
                    Date = new DateOnly(2025, 1, 15),
                    Rates = new Dictionary<string, decimal> { ["EUR"] = 0.92m }
                });

            MediatorMock
                .Setup(m => m.Send(It.IsAny<GetLatestExchangeRateQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(successResponse);

            _builderMock
                .Setup(b => b.Build(It.IsAny<GetLatestExchangeRateQueryResponse>()))
                .Returns(new OkObjectResult(successResponse.Data));

            _factoryMock
                .Setup(f => f.Create(It.IsAny<GetLatestExchangeRateQueryResponse>()))
                .Returns(_builderMock.Object);

            return this;
        }

        public TestBuilder SetupMediatorNotFound()
        {
            var notFoundResponse = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.NotFound);

            MediatorMock
                .Setup(m => m.Send(It.IsAny<GetLatestExchangeRateQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(notFoundResponse);

            _builderMock
                .Setup(b => b.Build(It.IsAny<GetLatestExchangeRateQueryResponse>()))
                .Returns(new NotFoundObjectResult(new ProblemDetails()));

            _factoryMock
                .Setup(f => f.Create(It.IsAny<GetLatestExchangeRateQueryResponse>()))
                .Returns(_builderMock.Object);

            return this;
        }

        public TestBuilder SetupMediatorGenericFailure()
        {
            var failureResponse = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.Generic);

            MediatorMock
                .Setup(m => m.Send(It.IsAny<GetLatestExchangeRateQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(failureResponse);

            _builderMock
                .Setup(b => b.Build(It.IsAny<GetLatestExchangeRateQueryResponse>()))
                .Returns(new ObjectResult(new ProblemDetails()) { StatusCode = StatusCodes.Status500InternalServerError });

            _factoryMock
                .Setup(f => f.Create(It.IsAny<GetLatestExchangeRateQueryResponse>()))
                .Returns(_builderMock.Object);

            return this;
        }

        public LatestExchangeRatesEndpoint Build()
            => new(MediatorMock.Object, _factoryMock.Object);
    }
}
