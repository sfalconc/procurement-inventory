using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Procurement.Domain.Entities;
using Procurement.Infrastructure.Persistence;
using Procurement.IntegrationTests.Common.Factories;

namespace Procurement.IntegrationTests.Common;

public static class TestDataSeeder
{
    public static async Task<(Supplier supplier, Product product)> SeedAsync(
        CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ProcurementDbContext>();


        var category = CategoryFactory.Create();

        context.Categories.Add(category);

        await context.SaveChangesAsync();


        var supplier = SupplierFactory.Create();

        var product = ProductFactory.Create(
            category.Id);


        context.Suppliers.Add(supplier);
        context.Products.Add(product);


        await context.SaveChangesAsync();


        return (supplier, product);
    }
}