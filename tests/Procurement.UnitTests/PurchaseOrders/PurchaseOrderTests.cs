using Procurement.Domain.Entities;
using Procurement.Domain.Enums;

namespace Procurement.UnitTests.PurchaseOrders;

public class PurchaseOrderTests
{
    [Fact]
    public void SetStatus_ShouldMoveFromDraftToPendingApproval()
    {
        var order = new PurchaseOrder(
            Guid.NewGuid());


        order.SetStatus(
            PurchaseOrderStatus.PendingApproval);


        Assert.Equal(
            PurchaseOrderStatus.PendingApproval,
            order.Status);
    }


    [Fact]
    public void SetStatus_ShouldThrow_WhenCompletingDraftOrder()
    {
        var order = new PurchaseOrder(
            Guid.NewGuid());


        Assert.Throws<InvalidOperationException>(
            () =>
            {
                order.SetStatus(
                    PurchaseOrderStatus.Completed);
            });
    }
}