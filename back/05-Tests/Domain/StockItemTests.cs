using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Tests.Domain;

public class StockItemTests
{
    private static readonly DateOnly Today = new(2026, 10, 12);

    [Fact]
    public void IsCaExpiredOn_ExpiryBeforeToday_IsExpired()
    {
        Assert.True(new StockItem { CaExpiryDate = Today.AddDays(-1) }.IsCaExpiredOn(Today));
    }

    [Fact]
    public void IsCaExpiredOn_ExpiryToday_IsStillValid()
    {
        Assert.False(new StockItem { CaExpiryDate = Today }.IsCaExpiredOn(Today));
    }

    [Fact]
    public void IsCaExpiredOn_WithoutCa_IsNotExpired()
    {
        Assert.False(new StockItem().IsCaExpiredOn(Today));
    }

    [Fact]
    public void IssueAsPpe_CopiesNameAndCaAsTheyAreToday()
    {
        var item = new StockItem { Id = 5, Name = "Botina de segurança", Ca = "12345", Unit = "par" };

        var issued = item.IssueAsPpe(2m, Today);
        item.Name = "Botina renomeada";
        item.Ca = "99999";

        Assert.Equal("Botina de segurança", issued.ItemNameSnapshot);
        Assert.Equal("12345", issued.CaSnapshot);
        Assert.Equal(5, issued.StockItemId);
        Assert.Equal(2m, issued.Quantity);
        Assert.Equal(PpeItemStatus.Issued, issued.Status);
    }
}
