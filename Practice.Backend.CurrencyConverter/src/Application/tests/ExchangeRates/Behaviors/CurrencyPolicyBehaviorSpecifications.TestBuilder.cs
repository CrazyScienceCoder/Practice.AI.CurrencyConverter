using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.Behaviors;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;
using Practice.Backend.CurrencyConverter.Domain.CurrencyPolicy;
using Practice.Backend.CurrencyConverter.Domain.Exceptions;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Application.Tests.ExchangeRates.Behaviors;

public partial class CurrencyPolicyBehaviorSpecifications
{
    private class TestBuilder
    {
        public readonly Mock<ICurrencyPolicy> CurrencyPolicyMock = new();
        private readonly Mock<ILogger<CurrencyPolicyBehavior<GetLatestExchangeRateQuery, GetLatestExchangeRateQueryResponse>>> _loggerMock = new();

        public bool NextDelegateWasCalled { get; private set; }

        public readonly GetLatestExchangeRateQuery DefaultRequest = new()
        {
            BaseCurrency = "USD",
            Provider = ExchangeRateProvider.Frankfurter
        };

        public readonly GetLatestExchangeRateQuery MultiCurrencyRequest = new()
        {
            BaseCurrency = "USD",
            Provider = ExchangeRateProvider.Frankfurter
        };

        public TestBuilder SetupAllCurrenciesAllowed()
        {
            CurrencyPolicyMock
                .Setup(p => p.EnsureAllowed(It.IsAny<Currency>()))
                .Returns(new Success())
                .Verifiable(Times.AtLeastOnce);

            return this;
        }

        public TestBuilder SetupForbiddenCurrency()
        {
            CurrencyPolicyMock
                .Setup(p => p.EnsureAllowed(It.IsAny<Currency>()))
                .Returns(Error.Forbidden("currency.not_allowed", "Currency USD is not allowed."))
                .Verifiable(Times.AtLeastOnce);

            return this;
        }

        public TestBuilder SetupMultipleForbiddenCurrencies()
        {
            CurrencyPolicyMock
                .Setup(p => p.EnsureAllowed(It.IsAny<Currency>()))
                .Returns(Error.Forbidden("currency.not_allowed", "Currency is not allowed."))
                .Verifiable(Times.AtLeastOnce);

            return this;
        }

        public TestBuilder SetupPolicyThrowsDomainValidationException()
        {
            CurrencyPolicyMock
                .Setup(p => p.EnsureAllowed(It.IsAny<Currency>()))
                .Throws(new InvalidCurrencyCodeException("Invalid currency code during policy check", "currency"));

            return this;
        }

        public TestBuilder SetupPolicyThrowsUnexpectedException()
        {
            CurrencyPolicyMock
                .Setup(p => p.EnsureAllowed(It.IsAny<Currency>()))
                .Throws(new Exception("Unexpected policy failure"));

            return this;
        }

        public RequestHandlerDelegate<GetLatestExchangeRateQueryResponse> CreateSuccessNextDelegate()
        {
            return _ =>
            {
                NextDelegateWasCalled = true;
                return Task.FromResult(GetLatestExchangeRateQueryResponse.Success(default, "Next delegate executed"));
            };
        }

        public CurrencyPolicyBehavior<GetLatestExchangeRateQuery, GetLatestExchangeRateQueryResponse> Build()
            => new(CurrencyPolicyMock.Object, _loggerMock.Object);
    }
}
