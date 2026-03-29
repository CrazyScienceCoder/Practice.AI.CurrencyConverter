using Practice.Backend.CurrencyConverter.Domain.Types.Validators;

namespace Practice.Backend.CurrencyConverter.Domain.Tests.Types.Validators;

public sealed class ExchangeDateValidatorSpecifications
{
    [Fact]
    public void IsFutureUtcDate_DateIsInFuture_ReturnsTrue()
    {
        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);

        var result = tomorrow.IsFutureUtcDate();

        result.Should().BeTrue();
    }

    [Fact]
    public void IsFutureUtcDate_DateIsToday_ReturnsFalse()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var result = today.IsFutureUtcDate();

        result.Should().BeFalse();
    }

    [Fact]
    public void IsFutureUtcDate_DateIsInPast_ReturnsFalse()
    {
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

        var result = yesterday.IsFutureUtcDate();

        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidDate_DateIsValid_ReturnsTrue()
    {
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

        var result = yesterday.IsValidDate();

        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidDate_DateIsMinValue_ReturnsFalse()
    {
        var result = DateOnly.MinValue.IsValidDate();

        result.Should().BeFalse();
    }
}
