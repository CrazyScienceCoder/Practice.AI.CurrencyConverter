using Practice.Backend.CurrencyConverter.Domain.Exceptions;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Domain.Tests.Types;

public sealed class AmountSpecifications
{
    [Theory]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(100.50)]
    public void Constructor_AmountIsPositive_CreatesAmount(decimal value)
    {
        var amount = new Amount(value);

        amount.Value.Should().Be(value);
    }

    [Fact]
    public void Constructor_AmountIsZero_ThrowsInvalidAmountException()
    {
        var act = () => new Amount(0m);

        act.Should().ThrowExactly<InvalidAmountException>()
            .Which.ParamName.Should().Be("amount");
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1)]
    [InlineData(-999.99)]
    public void Constructor_AmountIsNegative_ThrowsInvalidAmountException(decimal value)
    {
        var act = () => new Amount(value);

        act.Should().ThrowExactly<InvalidAmountException>()
            .Which.ParamName.Should().Be("amount");
    }

    [Fact]
    public void Constructor_AmountIsZero_ThrowsWithDescriptiveMessage()
    {
        var act = () => new Amount(0m);

        act.Should().ThrowExactly<InvalidAmountException>()
            .Which.Message.Should().Contain("Amount cannot be negative or Zero.");
    }

    [Fact]
    public void Create_AmountIsValid_ReturnsAmount()
    {
        var amount = Amount.Create(42.5m);

        amount.Value.Should().Be(42.5m);
    }

    [Fact]
    public void ImplicitOperator_ToDecimal_ReturnsUnderlyingValue()
    {
        var amount = new Amount(10m);

        decimal result = amount;

        result.Should().Be(10m);
    }

    [Fact]
    public void ImplicitOperator_FromDecimal_CreatesAmount()
    {
        Amount amount = 25.75m;

        amount.Value.Should().Be(25.75m);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        var a1 = new Amount(10m);
        var a2 = new Amount(10m);

        a1.Should().Be(a2);
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        var a1 = new Amount(10m);
        var a2 = new Amount(20m);

        a1.Should().NotBe(a2);
    }
}
