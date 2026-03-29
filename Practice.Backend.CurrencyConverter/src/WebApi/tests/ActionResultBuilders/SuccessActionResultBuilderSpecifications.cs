using Microsoft.AspNetCore.Mvc;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Builders;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.ActionResultBuilders;

public sealed class SuccessActionResultBuilderSpecifications
{
    private readonly SuccessActionResultBuilder _sut = new();

    [Fact]
    public void CanHandle_ResultIsSuccess_ReturnsTrue()
    {
        var result = GetLatestExchangeRateQueryResponse.Success(BuildQueryResult());

        _sut.CanHandle(result).Should().BeTrue();
    }

    [Fact]
    public void CanHandle_ResultIsFailure_ReturnsFalse()
    {
        var result = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.NotFound);

        _sut.CanHandle(result).Should().BeFalse();
    }

    [Fact]
    public void Build_SuccessResult_ReturnsOkObjectResult()
    {
        var data = BuildQueryResult();
        var result = GetLatestExchangeRateQueryResponse.Success(data);

        var actionResult = _sut.Build(result);

        actionResult.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void Build_SuccessResult_ReturnsOkObjectResultWithResultData()
    {
        var data = BuildQueryResult();
        var result = GetLatestExchangeRateQueryResponse.Success(data);

        var actionResult = (OkObjectResult)_sut.Build(result);

        actionResult.Value.Should().BeSameAs(data);
    }

    [Fact]
    public void Build_SuccessResultWithNullData_ReturnsOkObjectResultWithNullValue()
    {
        var result = GetLatestExchangeRateQueryResponse.Success(data: null);

        var actionResult = (OkObjectResult)_sut.Build(result);

        actionResult.Value.Should().BeNull();
    }

    private static GetLatestExchangeRateQueryResult BuildQueryResult()
        => new()
        {
            Amount = 1m,
            Base = "USD",
            Date = new DateOnly(2025, 1, 15),
            Rates = new Dictionary<string, decimal> { ["EUR"] = 0.92m }
        };
}
