using Practice.Backend.CurrencyConverter.Application.Shared;

namespace Practice.Backend.CurrencyConverter.Application.Tests.ExchangeRates.Behaviors;

public partial class CurrencyPolicyBehaviorSpecifications
{
    [Fact]
    public async Task Handle_AllCurrenciesAllowed_CallsNextDelegateAndReturnsItsResult()
    {
        var testBuilder = new TestBuilder()
            .SetupAllCurrenciesAllowed();

        var behavior = testBuilder.Build();

        var result = await behavior.Handle(
            testBuilder.DefaultRequest,
            testBuilder.CreateSuccessNextDelegate(),
            TestContext.Current.CancellationToken);

        testBuilder.CurrencyPolicyMock.Verify();
        testBuilder.NextDelegateWasCalled.Should().BeTrue();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ForbiddenCurrency_ReturnsNotAllowedWithoutCallingNext()
    {
        var testBuilder = new TestBuilder()
            .SetupForbiddenCurrency();

        var behavior = testBuilder.Build();

        var result = await behavior.Handle(
            testBuilder.DefaultRequest,
            testBuilder.CreateSuccessNextDelegate(),
            TestContext.Current.CancellationToken);

        testBuilder.CurrencyPolicyMock.Verify();
        testBuilder.NextDelegateWasCalled.Should().BeFalse();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotAllowed);
        result.Message.Should().Contain("not allowed");
    }

    [Fact]
    public async Task Handle_MultipleForbiddenCurrencies_ReturnsNotAllowedWithCombinedDescriptions()
    {
        var testBuilder = new TestBuilder()
            .SetupMultipleForbiddenCurrencies();

        var behavior = testBuilder.Build();

        var result = await behavior.Handle(
            testBuilder.MultiCurrencyRequest,
            testBuilder.CreateSuccessNextDelegate(),
            TestContext.Current.CancellationToken);

        testBuilder.CurrencyPolicyMock.Verify();
        testBuilder.NextDelegateWasCalled.Should().BeFalse();
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotAllowed);
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_DomainValidationExceptionThrownByPolicy_ReturnsValidationErrorFailure()
    {
        var testBuilder = new TestBuilder()
            .SetupPolicyThrowsDomainValidationException();

        var behavior = testBuilder.Build();

        var result = await behavior.Handle(
            testBuilder.DefaultRequest,
            testBuilder.CreateSuccessNextDelegate(),
            TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.ValidationError);
    }

    [Fact]
    public async Task Handle_UnexpectedExceptionThrownByPolicy_ReturnsGenericFailure()
    {
        var testBuilder = new TestBuilder()
            .SetupPolicyThrowsUnexpectedException();

        var behavior = testBuilder.Build();

        var result = await behavior.Handle(
            testBuilder.DefaultRequest,
            testBuilder.CreateSuccessNextDelegate(),
            TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Generic);
    }
}
