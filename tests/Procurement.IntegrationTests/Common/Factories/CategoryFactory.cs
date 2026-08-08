using Bogus;
using Procurement.Domain.Entities;

namespace Procurement.IntegrationTests.Common.Factories;

public static class CategoryFactory
{
    public static Category Create()
    {
        return new Category($"Category {Guid.NewGuid()}",
                "Test category");
    }
}