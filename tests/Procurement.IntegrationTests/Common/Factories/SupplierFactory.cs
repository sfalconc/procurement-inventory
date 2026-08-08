using Bogus;
using Procurement.Domain.Entities;

namespace Procurement.IntegrationTests.Common.Factories;

public static class SupplierFactory
{
    public static Supplier Create()
    {
        return new Supplier(
            $"SUP-{Guid.NewGuid()}",
            $"Supplier {Guid.NewGuid()}",
            "123456789",
            "Test Contact",
            "test@test.com",
            "999999999");
    }
}