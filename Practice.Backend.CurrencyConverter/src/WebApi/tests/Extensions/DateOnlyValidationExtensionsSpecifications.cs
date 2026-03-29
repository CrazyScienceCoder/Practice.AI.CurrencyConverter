using FluentValidation;
using Practice.Backend.CurrencyConverter.WebApi.Extensions;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Extensions;

public sealed class DateOnlyValidationExtensionsSpecifications
{
    private sealed class TestModel { public DateOnly? Date { get; init; } }

    private sealed class TestValidator : AbstractValidator<TestModel>
    {
        public TestValidator()
        {
            RuleFor(x => x.Date).MustBeValidDateOnly();
        }
    }

    private readonly TestValidator _sut = new();

    [Fact]
    public async Task MustBeValidDateOnly_PastDate_PassesValidation()
    {
        var model = new TestModel { Date = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1) };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task MustBeValidDateOnly_TodayDate_PassesValidation()
    {
        var model = new TestModel { Date = DateOnly.FromDateTime(DateTime.UtcNow) };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task MustBeValidDateOnly_NullDate_FailsWithRequiredMessage()
    {
        var model = new TestModel { Date = null };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("required"));
    }

    [Fact]
    public async Task MustBeValidDateOnly_FutureDate_FailsWithFutureMessage()
    {
        var model = new TestModel { Date = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1) };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("future"));
    }

    [Fact]
    public async Task MustBeValidDateOnly_FarPastDate_PassesValidation()
    {
        var model = new TestModel { Date = new DateOnly(2000, 1, 1) };

        var result = await _sut.ValidateAsync(model, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }
}
