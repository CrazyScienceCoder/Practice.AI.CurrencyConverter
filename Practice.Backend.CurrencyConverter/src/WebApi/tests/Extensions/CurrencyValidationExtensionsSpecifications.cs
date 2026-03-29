using FluentValidation;
using Practice.Backend.CurrencyConverter.WebApi.Extensions;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Extensions;

public sealed class CurrencyValidationExtensionsSpecifications
{
    private sealed class TestModel { public string Currency { get; init; } = string.Empty; }

    private sealed class TestValidator : AbstractValidator<TestModel>
    {
        public TestValidator()
        {
            RuleFor(x => x.Currency).MustBeValidCurrency();
        }
    }

    private readonly TestValidator _sut = new();

    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GBP")]
    [InlineData("JPY")]
    [InlineData("CHF")]
    public async Task MustBeValidCurrency_KnownIso4217Code_PassesValidation(string code)
    {
        var model = new TestModel { Currency = code };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task MustBeValidCurrency_EmptyString_FailsWithRequiredMessage()
    {
        var model = new TestModel { Currency = "" };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("required"));
    }

    [Theory]
    [InlineData("INVALID")]
    [InlineData("XY")]
    [InlineData("12345")]
    [InlineData("ABC")]
    public async Task MustBeValidCurrency_InvalidCode_FailsWithIso4217Message(string code)
    {
        var model = new TestModel { Currency = code };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("ISO 4217"));
    }

    [Theory]
    [InlineData("usd")]
    [InlineData("eur")]
    [InlineData("Usd")]
    public async Task MustBeValidCurrency_ValidCodeLowercase_PassesValidation(string code)
    {
        var model = new TestModel { Currency = code };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }
}
