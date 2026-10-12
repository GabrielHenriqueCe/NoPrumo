using NoPrumo.Domain.Entities;

namespace NoPrumo.Tests.Domain;

public class StockCategoryTests
{
    [Theory]
    [InlineData(true, false, false)]  // material de consumo: saldo por obra
    [InlineData(false, true, false)]  // ferramenta: vai e volta
    [InlineData(false, false, true)]  // EPI: nem uma coisa nem outra
    public void IsPpe_OnlyWhenNeitherFlagIsSet(bool tracksProjectBalance, bool requiresReturn, bool expected)
    {
        var category = new StockCategory { TracksProjectBalance = tracksProjectBalance, RequiresReturn = requiresReturn };

        Assert.Equal(expected, category.IsPpe());
    }
}
