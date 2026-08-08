using Procurement.Domain.Entities;
using Procurement.Domain.Enums;

namespace Procurement.UnitTests.PurchaseOrders;

public class PurchaseOrderTests
{
    [Fact]
    public void Approve_ShouldMoveFromDraftToApproved()
    {
        var order = new PurchaseOrder(
            Guid.NewGuid());

        order.Approve();

        Assert.Equal(
            PurchaseOrderStatus.Approved,
            order.Status);
    }

    [Fact]
    public void MarkAsCompleted_ShouldThrow_WhenOrderIsDraft()
    {
        var order = new PurchaseOrder(
            Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(
            () =>
            {
                order.MarkAsCompleted();
            });
    }
}