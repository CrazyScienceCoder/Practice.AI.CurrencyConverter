using FluentValidation;
using Practice.Backend.CurrencyConverter.WebApi.Extensions;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Extensions;

public sealed class ProviderValidationExtensionsSpecifications
{
    private sealed class TestModel { public string? Provider { get; init; } }

    private sealed class TestValidator : AbstractValidator<TestModel>
    {
        public TestValidator()
        {
            RuleFor(x => x.Provider).MustBeValidProvider();
        }
    }

    private readonly TestValidator _sut = new();

    [Fact]
    public async Task MustBeValidProvider_NullValue_PassesValidation()
    {
        var model = new TestModel { Provider = null };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task MustBeValidProvider_ValidProviderName_PassesValidation()
    {
        var model = new TestModel { Provider = "Frankfurter" };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task MustBeValidProvider_ValidProviderNameLowercase_PassesValidation()
    {
        var model = new TestModel { Provider = "frankfurter" };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task MustBeValidProvider_ValidProviderNameUppercase_PassesValidation()
    {
        var model = new TestModel { Provider = "FRANKFURTER" };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task MustBeValidProvider_InvalidProviderName_FailsValidation()
    {
        var model = new TestModel { Provider = "NonExistentProvider" };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task MustBeValidProvider_InvalidProviderName_FailsWithExpectedMessage()
    {
        var model = new TestModel { Provider = "Unknown" };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Provider"));
    }

    [Theory]
    [InlineData("SomeRandomString")]
    [InlineData("Binance")]
    [InlineData("Fixer")]
    public async Task MustBeValidProvider_UnknownProviderName_FailsValidation(string provider)
    {
        var model = new TestModel { Provider = provider };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
    }
}
