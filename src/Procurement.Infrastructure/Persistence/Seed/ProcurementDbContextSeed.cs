using Bogus;
using Microsoft.EntityFrameworkCore;
using Procurement.Domain.Entities;

namespace Procurement.Infrastructure.Persistence.Seed;

public static class ProcurementDbContextSeed
{
    public static async Task SeedAsync(
        ProcurementDbContext context)
    {
        if (await context.Categories.AnyAsync())
        {
            return;
        }

        var random = new Random();


        // Categories

        var categories = new List<Category>
        {
            new(
                "Electronics",
                "Electronic components and devices"),

            new(
                "Office Supplies",
                "Office and administrative materials"),

            new(
                "Industrial Equipment",
                "Machinery and industrial tools"),

            new(
                "Safety Equipment",
                "Workplace safety products"),

            new(
                "Packaging",
                "Packaging and shipping materials")
        };


        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();



        // Suppliers

        var supplierFaker = new Faker<Supplier>()
            .CustomInstantiator(f =>
                new Supplier(
                    $"SUP-{f.Random.Number(1000,9999)}",
                    f.Company.CompanyName(),
                    f.Random.Replace("##########"),
                    f.Name.FullName(),
                    f.Internet.Email(),
                    f.Phone.PhoneNumber()));

        var suppliers = supplierFaker.Generate(10);


        await context.Suppliers.AddRangeAsync(suppliers);
        await context.SaveChangesAsync();



        // Products

        var productFaker = new Faker<Product>()
            .CustomInstantiator(f =>
                new Product(
                    $"PRD-{f.Random.Number(10000,99999)}",
                    f.Commerce.ProductName(),
                    f.Commerce.ProductDescription(),
                    f.PickRandom(categories).Id));


        var products = productFaker.Generate(30);


        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();



        // Inventory

        var inventories = products
            .Select(product =>
            {
                var inventory = new Inventory(
                    product.Id,
                    random.Next(5,50),
                    random.Next(10,500));

                inventory.AdjustStock(
                    random.Next(20,200));

                return inventory;

            })
            .ToList();


        await context.Inventories.AddRangeAsync(inventories);
        await context.SaveChangesAsync();



        // Purchase Orders

        var purchaseOrders = new List<PurchaseOrder>();


        foreach (var supplier in suppliers.Take(5))
        {
            var order = new PurchaseOrder(
                supplier.Id);


            var orderProducts = products
                .OrderBy(_ => random.Next())
                .Take(3);


            foreach (var product in orderProducts)
            {
                order.AddLine(
                    product.Id,
                    random.Next(1,20),
                    random.Next(10,500));
            }


            if (random.Next(0,2) == 1)
            {
                order.Approve();
            }


            purchaseOrders.Add(order);
        }


        await context.PurchaseOrders.AddRangeAsync(
            purchaseOrders);

        await context.SaveChangesAsync();
    }
}