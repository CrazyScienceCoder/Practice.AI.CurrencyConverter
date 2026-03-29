using Practice.Backend.CurrencyConverter.Domain.CurrencyPolicy;

namespace Practice.Backend.CurrencyConverter.Domain.Tests.CurrencyPolicy;

public sealed class ForbiddenCurrenciesSpecifications
{
    [Fact]
    public void MXN_CodeAndDescription_AreCorrect()
    {
        ForbiddenCurrencies.MXN.Code.Should().Be("MXN");
        ForbiddenCurrencies.MXN.Description.Should().Be("Mexican Peso");
    }

    [Fact]
    public void PLN_CodeAndDescription_AreCorrect()
    {
        ForbiddenCurrencies.PLN.Code.Should().Be("PLN");
        ForbiddenCurrencies.PLN.Description.Should().Be("Polish Zloty");
    }

    [Fact]
    public void THB_CodeAndDescription_AreCorrect()
    {
        ForbiddenCurrencies.THB.Code.Should().Be("THB");
        ForbiddenCurrencies.THB.Description.Should().Be("Thai Baht");
    }

    [Fact]
    public void TRY_CodeAndDescription_AreCorrect()
    {
        ForbiddenCurrencies.TRY.Code.Should().Be("TRY");
        ForbiddenCurrencies.TRY.Description.Should().Be("Turkish Lira");
    }

    [Fact]
    public void List_Always_ContainsFourEntries()
    {
        ForbiddenCurrencies.List.Should().HaveCount(4);
    }

    [Fact]
    public void List_Always_ContainsAllForbiddenCodes()
    {
        var codes = ForbiddenCurrencies.List.Select(c => c.Code);

        codes.Should().BeEquivalentTo(["MXN", "PLN", "THB", "TRY"]);
    }

    [Theory]
    [InlineData(1, "MXN")]
    [InlineData(2, "PLN")]
    [InlineData(3, "THB")]
    [InlineData(4, "TRY")]
    public void FromValue_ValidId_ReturnsForbiddenCurrencyWithMatchingCode(int id, string expectedCode)
    {
        var currency = ForbiddenCurrencies.FromValue(id);

        currency.Code.Should().Be(expectedCode);
    }

    [Theory]
    [InlineData("MXN")]
    [InlineData("PLN")]
    [InlineData("THB")]
    [InlineData("TRY")]
    public void FromName_ValidCode_ReturnsForbiddenCurrencyWithMatchingCode(string code)
    {
        var currency = ForbiddenCurrencies.FromName(code);

        currency.Code.Should().Be(code);
    }
}
