using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Clients;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Tests.Clients;

public sealed class QueryBuilderSpecifications
{
    [Fact]
    public void Create_WithBaseCurrency_SetsBaseQueryParameter()
    {
        var query = QueryBuilder.Create("USD").Build();

        query.Should().ContainKey("base").WhoseValue.Should().Be("USD");
    }

    [Theory]
    [InlineData("EUR")]
    [InlineData("USD")]
    [InlineData("GBP")]
    public void Create_WithVariousBaseCurrencies_StoresCorrectBase(string baseCurrency)
    {
        var query = QueryBuilder.Create(baseCurrency).Build();

        query["base"].Should().Be(baseCurrency);
    }

    [Fact]
    public void WithAmount_WholeNumber_FormatsWithoutDecimalPlaces()
    {
        var query = QueryBuilder.Create("EUR").WithAmount(1).Build();

        query["amount"].Should().Be("1");
    }

    [Theory]
    [InlineData(1.5, "1.5")]
    [InlineData(100.1234, "100.1234")]
    [InlineData(0.0001, "0.0001")]
    [InlineData(999.99, "999.99")]
    public void WithAmount_DecimalNumber_FormatsToMaxFourDecimalPlaces(double amount, string expected)
    {
        var query = QueryBuilder.Create("EUR").WithAmount(amount).Build();

        query["amount"].Should().Be(expected);
    }

    [Fact]
    public void WithAmount_DecimalValue_UsesInvariantCultureDecimalSeparator()
    {
        var query = QueryBuilder.Create("EUR").WithAmount(1.5).Build();

        query["amount"].Should().Be("1.5");
        query["amount"].Should().NotContain(",");
    }

    [Fact]
    public void WithAmount_SetsAmountQueryParameter()
    {
        var query = QueryBuilder.Create("EUR").WithAmount(42).Build();

        query.Should().ContainKey("amount");
    }

    [Fact]
    public void WithSymbols_WithMultipleSymbols_AddsCommaSeparatedValue()
    {
        var query = QueryBuilder.Create("EUR").WithSymbols(["USD", "GBP", "JPY"]).Build();

        query["symbols"].Should().Be("USD,GBP,JPY");
    }

    [Fact]
    public void WithSymbols_WithSingleSymbol_AddsSingleSymbolValue()
    {
        var query = QueryBuilder.Create("EUR").WithSymbols(["USD"]).Build();

        query["symbols"].Should().Be("USD");
    }

    [Fact]
    public void WithSymbols_WithNull_DoesNotAddSymbolsKey()
    {
        var query = QueryBuilder.Create("EUR").WithSymbols(null).Build();

        query.Should().NotContainKey("symbols");
    }

    [Fact]
    public void WithSymbols_WithEmptyArray_DoesNotAddSymbolsKey()
    {
        var query = QueryBuilder.Create("EUR").WithSymbols([]).Build();

        query.Should().NotContainKey("symbols");
    }

    [Fact]
    public void Build_Always_ReturnsIReadOnlyDictionary()
    {
        var query = QueryBuilder.Create("EUR").Build();

        query.Should().BeAssignableTo<IReadOnlyDictionary<string, string?>>();
    }

    [Fact]
    public void Build_WithNoOptionalParameters_ContainsOnlyBaseKey()
    {
        var query = QueryBuilder.Create("EUR").Build();

        query.Should().ContainSingle()
            .Which.Key.Should().Be("base");
    }

    [Fact]
    public void WithAmount_ReturnsQueryBuilderForFluentChaining()
    {
        var builder = QueryBuilder.Create("EUR");

        var result = builder.WithAmount(1.5);

        result.Should().BeSameAs(builder);
    }

    [Fact]
    public void WithSymbols_ReturnsQueryBuilderForFluentChaining()
    {
        var builder = QueryBuilder.Create("EUR");

        var result = builder.WithSymbols(["USD"]);

        result.Should().BeSameAs(builder);
    }

    [Fact]
    public void Build_WithAllParameters_ContainsBaseAmountAndSymbolsKeys()
    {
        var query = QueryBuilder.Create("EUR")
            .WithAmount(100)
            .WithSymbols(["USD", "GBP"])
            .Build();

        query.Should().ContainKey("base");
        query.Should().ContainKey("amount");
        query.Should().ContainKey("symbols");
    }
}
