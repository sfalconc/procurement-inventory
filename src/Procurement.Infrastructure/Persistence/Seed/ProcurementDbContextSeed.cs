using Bogus;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Domain.Entities;
using Procurement.Domain.Enums;

namespace Procurement.Infrastructure.Persistence.Seed;

public static class ProcurementDbContextSeed
{
    public static async Task SeedAsync(
        ProcurementDbContext context,
        IPasswordHasher passwordHasher)
    {
        await SeedUserAsync(context, passwordHasher);

        if (await context.Categories.AnyAsync())
        {
            return;
        }

        var random = new Random();

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

        var supplierFaker = new Faker<Supplier>()
            .CustomInstantiator(f =>
                new Supplier(
                    $"SUP-{f.Random.Number(1000, 9999)}",
                    f.Company.CompanyName(),
                    f.Random.Replace("##########"),
                    f.Name.FullName(),
                    f.Internet.Email(),
                    f.Phone.PhoneNumber()));

        var suppliers = supplierFaker.Generate(10);

        await context.Suppliers.AddRangeAsync(suppliers);
        await context.SaveChangesAsync();

        var productFaker = new Faker<Product>()
            .CustomInstantiator(f =>
                new Product(
                    $"PRD-{f.Random.Number(10000, 99999)}",
                    f.Commerce.ProductName(),
                    f.Commerce.ProductDescription(),
                    f.PickRandom(categories).Id));

        var products = productFaker.Generate(30);

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        var inventories = products
            .Select(product =>
            {
                var inventory = new Inventory(
                    product.Id,
                    random.Next(5, 50),
                    random.Next(10, 500));

                inventory.AdjustStock(
                    random.Next(20, 200));

                return inventory;
            })
            .ToList();

        await context.Inventories.AddRangeAsync(inventories);
        await context.SaveChangesAsync();

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
                    random.Next(1, 20),
                    random.Next(10, 500));
            }

            if (random.Next(0, 2) == 1)
            {
                order.Approve();
            }

            purchaseOrders.Add(order);
        }

        await context.PurchaseOrders.AddRangeAsync(
            purchaseOrders);

        await context.SaveChangesAsync();
    }

    private static async Task SeedUserAsync(
        ProcurementDbContext context,
        IPasswordHasher passwordHasher)
    {
        await CreateUserIfNotExistsAsync(
            context,
            passwordHasher,
            "superuser",
            "SuperUser123!",
            UserRole.SuperUser);

        await CreateUserIfNotExistsAsync(
            context,
            passwordHasher,
            "purchaser",
            "Purchaser123!",
            UserRole.Purchaser);
    }

    private static async Task CreateUserIfNotExistsAsync(
        ProcurementDbContext context,
        IPasswordHasher passwordHasher,
        string username,
        string password,
        UserRole role)
    {
        var userExists = await context.Users
            .AnyAsync(u => u.Username == username);

        if (userExists)
        {
            return;
        }

        var passwordHash =
            passwordHasher.HashPassword(password);

        var user = new User(
            username,
            passwordHash,
            role);

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }
}