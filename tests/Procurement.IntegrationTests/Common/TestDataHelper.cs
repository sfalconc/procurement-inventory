using Microsoft.Extensions.DependencyInjection;

using Procurement.Domain.Entities;
using Procurement.Infrastructure.Persistence;

namespace Procurement.IntegrationTests.Common;

public static class TestDataHelper
{
    public static async Task<(Supplier supplier, Product product)> CreateBasicPurchaseData(
        CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ProcurementDbContext>();


        var category = new Category("Test Category",
            "Integration Test Category");


        var supplier = new Supplier(
            "SUP-001",
            "Test Supplier",
            "123456789",
            "Test Contact",
            "test@supplier.com",
            "999999999");


        context.Categories.Add(category);

        await context.SaveChangesAsync();


        var product = new Product(
            "PROD-001",
            "Test Product",
            "Integration Test Product",
            category.Id);


        context.Suppliers.Add(supplier);
        context.Products.Add(product);


        await context.SaveChangesAsync();


        return (supplier, product);
    }
}