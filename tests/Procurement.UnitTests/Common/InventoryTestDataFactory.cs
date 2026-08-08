using Procurement.Domain.Entities;

using InventoryEntity = Procurement.Domain.Entities.Inventory;

namespace Procurement.UnitTests.Common;

public static class InventoryTestDataFactory
{
    public static Category CreateCategory()
    {
        return new Category("Test Category",
            "Category for tests");
    }

    public static Product CreateProduct(Guid categoryId)
    {
        return new Product(
            "PROD-001",
            "Test Product",
            "Product for tests",
            categoryId);
    }


    public static InventoryEntity CreateInventory(
        Guid productId,
        int quantity = 100,
        int minimumStock = 10,
        decimal averageUnitCost = 25)
    {
        var inventory = new InventoryEntity(
            productId,
            quantity,
            averageUnitCost);

        inventory.SetMinimumStock(minimumStock);

        return inventory;
    }
}