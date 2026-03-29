using Practice.Backend.CurrencyConverter.Domain.Exceptions;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Domain.Tests.Types;

public sealed class ExchangeDateSpecifications
{
    [Fact]
    public void Constructor_DateIsInPast_CreatesExchangeDate()
    {
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

        var exchangeDate = new ExchangeDate(yesterday);

        exchangeDate.Value.Should().Be(yesterday);
    }

    [Fact]
    public void Constructor_DateIsToday_CreatesExchangeDate()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var exchangeDate = new ExchangeDate(today);

        exchangeDate.Value.Should().Be(today);
    }

    [Fact]
    public void Constructor_DateIsInFuture_ThrowsInvalidExchangeDateException()
    {
        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);

        var act = () => new ExchangeDate(tomorrow);

        act.Should().ThrowExactly<InvalidExchangeDateException>()
            .Which.Message.Should().Contain("Date cannot be in the future.");
    }

    [Fact]
    public void Constructor_DateIsInFuture_ThrowsWithCorrectParamName()
    {
        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);

        var act = () => new ExchangeDate(tomorrow);

        act.Should().ThrowExactly<InvalidExchangeDateException>()
            .Which.ParamName.Should().Be("dateUtc");
    }

    [Fact]
    public void Constructor_DateIsMinValue_ThrowsInvalidExchangeDateException()
    {
        var act = () => new ExchangeDate(DateOnly.MinValue);

        act.Should().ThrowExactly<InvalidExchangeDateException>()
            .Which.Message.Should().Contain("Date is invalid.");
    }

    [Fact]
    public void Constructor_DateIsMinValue_ThrowsWithCorrectParamName()
    {
        var act = () => new ExchangeDate(DateOnly.MinValue);

        act.Should().ThrowExactly<InvalidExchangeDateException>()
            .Which.ParamName.Should().Be("dateUtc");
    }

    [Fact]
    public void Create_DateIsValid_ReturnsExchangeDate()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-30);

        var exchangeDate = ExchangeDate.Create(date);

        exchangeDate.Value.Should().Be(date);
    }

    [Fact]
    public void ImplicitOperator_ToDateOnly_ReturnsUnderlyingValue()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);
        var exchangeDate = new ExchangeDate(date);

        DateOnly result = exchangeDate;

        result.Should().Be(date);
    }

    [Fact]
    public void ImplicitOperator_FromDateOnly_CreatesExchangeDate()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

        ExchangeDate exchangeDate = date;

        exchangeDate.Value.Should().Be(date);
    }

    [Fact]
    public void Equals_SameDateValue_ReturnsTrue()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

        var d1 = new ExchangeDate(date);
        var d2 = new ExchangeDate(date);

        d1.Should().Be(d2);
    }

    [Fact]
    public void Equals_DifferentDateValues_ReturnsFalse()
    {
        var d1 = new ExchangeDate(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1));
        var d2 = new ExchangeDate(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-2));

        d1.Should().NotBe(d2);
    }
}
