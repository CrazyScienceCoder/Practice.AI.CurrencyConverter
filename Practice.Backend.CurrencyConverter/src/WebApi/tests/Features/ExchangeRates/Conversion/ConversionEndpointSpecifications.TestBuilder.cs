using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.CurrencyConversion;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Builders;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Factories;
using Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Conversion;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Features.ExchangeRates.Conversion;

public partial class ConversionEndpointSpecifications
{
    private class TestBuilder
    {
        public readonly Mock<IMediator> MediatorMock = new();
        private readonly Mock<IActionResultBuilderFactory> _factoryMock = new();
        private readonly Mock<IActionResultBuilder> _builderMock = new();

        public readonly ConversionRequest DefaultRequest = new()
        {
            BaseCurrency = "USD",
            ToCurrency = "EUR",
            Amount = 100m,
            Provider = null
        };

        public TestBuilder SetupMediatorSuccess()
        {
            var successResponse = GetCurrencyConversionQueryResponse.Success(
                new GetCurrencyConversionQueryResult
                {
                    Amount = 92m,
                    Base = "USD",
                    Date = new DateOnly(2025, 1, 15),
                    Rates = new Dictionary<string, decimal> { ["EUR"] = 92m }
                });

            MediatorMock
                .Setup(m => m.Send(It.IsAny<GetCurrencyConversionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(successResponse);

            _builderMock
                .Setup(b => b.Build(It.IsAny<GetCurrencyConversionQueryResponse>()))
                .Returns(new OkObjectResult(successResponse.Data));

            _factoryMock
                .Setup(f => f.Create(It.IsAny<GetCurrencyConversionQueryResponse>()))
                .Returns(_builderMock.Object);

            return this;
        }

        public TestBuilder SetupMediatorNotFound()
        {
            var notFoundResponse = GetCurrencyConversionQueryResponse.Failure(errorType: ErrorType.NotFound);

            MediatorMock
                .Setup(m => m.Send(It.IsAny<GetCurrencyConversionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(notFoundResponse);

            _builderMock
                .Setup(b => b.Build(It.IsAny<GetCurrencyConversionQueryResponse>()))
                .Returns(new NotFoundObjectResult(new ProblemDetails()));

            _factoryMock
                .Setup(f => f.Create(It.IsAny<GetCurrencyConversionQueryResponse>()))
                .Returns(_builderMock.Object);

            return this;
        }

        public TestBuilder SetupMediatorGenericFailure()
        {
            var failureResponse = GetCurrencyConversionQueryResponse.Failure(errorType: ErrorType.Generic);

            MediatorMock
                .Setup(m => m.Send(It.IsAny<GetCurrencyConversionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(failureResponse);

            _builderMock
                .Setup(b => b.Build(It.IsAny<GetCurrencyConversionQueryResponse>()))
                .Returns(new ObjectResult(new ProblemDetails()) { StatusCode = StatusCodes.Status500InternalServerError });

            _factoryMock
                .Setup(f => f.Create(It.IsAny<GetCurrencyConversionQueryResponse>()))
                .Returns(_builderMock.Object);

            return this;
        }

        public TestBuilder SetupMediatorNotAllowed()
        {
            var notAllowedResponse = GetCurrencyConversionQueryResponse.Failure(errorType: ErrorType.NotAllowed);

            MediatorMock
                .Setup(m => m.Send(It.IsAny<GetCurrencyConversionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(notAllowedResponse);

            _builderMock
                .Setup(b => b.Build(It.IsAny<GetCurrencyConversionQueryResponse>()))
                .Returns(new BadRequestObjectResult(new ProblemDetails()));

            _factoryMock
                .Setup(f => f.Create(It.IsAny<GetCurrencyConversionQueryResponse>()))
                .Returns(_builderMock.Object);

            return this;
        }

        public ConversionEndpoint Build()
            => new(MediatorMock.Object, _factoryMock.Object);
    }
}
