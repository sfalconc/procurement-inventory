using Procurement.Domain.Entities;

namespace Procurement.IntegrationTests.Common.Factories;

public static class ProductFactory
{
    public static Product Create(Guid categoryId)
    {
        return new Product(
            $"PROD-{Guid.NewGuid()}",
            $"Product {Guid.NewGuid()}",
            "Test product",
            categoryId);
    }
}