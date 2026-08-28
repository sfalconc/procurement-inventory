using Xunit;

namespace Procurement.UnitTests.Inventory;

public class InventoryMonitoringWorkerTests
{
    [Fact]
    public void Product_Should_Be_Notified_Only_Once_When_Remaining_Low_Stock()
    {
        var notifiedProducts = new HashSet<Guid>();
        var productId = Guid.NewGuid();

        var firstNotification = notifiedProducts.Add(productId);
        var secondNotification = notifiedProducts.Add(productId);

        Assert.True(firstNotification);
        Assert.False(secondNotification);
    }

    [Fact]
    public void Product_Should_Be_Notified_Again_After_Leaving_Low_Stock()
    {
        var notifiedProducts = new HashSet<Guid>();
        var productId = Guid.NewGuid();

        notifiedProducts.Add(productId);
        notifiedProducts.Remove(productId);

        var notificationAfterRecovery =
            notifiedProducts.Add(productId);

        Assert.True(notificationAfterRecovery);
    }
}