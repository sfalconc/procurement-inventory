using Procurement.Application.Products.Services;
using Procurement.UnitTests.Common;
using Procurement.Domain.Entities;
using Procurement.Application.Products.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Procurement.UnitTests.Products.Services;

public class ProductServiceTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _database = new SqliteTestDatabase();

        _service = new ProductService(
            _database.Context);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct_WhenCategoryExists()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronics",
            Description = "Test category"
        };

        _database.Context.Categories.Add(category);

        await _database.Context.SaveChangesAsync();


        var request = new CreateProductRequest
        {
            Code = "LAPTOP-001",
            Name = "Laptop Lenovo",
            Description = "Test product",
            CategoryId = category.Id
        };


        // Act
        var result = await _service.CreateAsync(
            request,
            CancellationToken.None);


        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);

        Assert.Equal(
            request.Code,
            result.Code);

        Assert.Equal(
            request.Name,
            result.Name);


        var productInDb = await _database.Context.Products
            .FirstOrDefaultAsync(
                p => p.Id == result.Id);


        Assert.NotNull(productInDb);

        Assert.Equal(
            "Laptop Lenovo",
            productInDb.Name);
    }    

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Code = "LAPTOP-002",
            Name = "Laptop Without Category",
            Description = "Invalid product",
            CategoryId = Guid.NewGuid()
        };


        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
            {
                await _service.CreateAsync(
                    request,
                    CancellationToken.None);
            });


        Assert.Equal(
            "Category does not exist.",
            exception.Message);


        var productsCount = await _database.Context.Products.CountAsync();

        Assert.Equal(
            0,
            productsCount);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronics"
        };

        var product = new Product(
            "PHONE-001",
            "iPhone",
            "Mobile phone",
            category.Id);

        _database.Context.Categories.Add(category);
        _database.Context.Products.Add(product);

        await _database.Context.SaveChangesAsync();


        // Act
        var result = await _service.GetByIdAsync(
            product.Id,
            CancellationToken.None);


        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            product.Id,
            result.Id);

        Assert.Equal(
            "PHONE-001",
            result.Code);

        Assert.Equal(
            "iPhone",
            result.Name);

        Assert.Equal(
            "Electronics",
            result.CategoryName);
    }    

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();


        // Act
        var result = await _service.GetByIdAsync(
            nonExistingId,
            CancellationToken.None);


        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnProducts()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronics"
        };

        var product1 = new Product(
            "PHONE-001",
            "iPhone",
            "Mobile phone",
            category.Id);

        var product2 = new Product(
            "LAPTOP-001",
            "Laptop Lenovo",
            "Notebook",
            category.Id);


        _database.Context.Categories.Add(category);

        _database.Context.Products.AddRange(
            product1,
            product2);

        await _database.Context.SaveChangesAsync();


        // Act
        var result = await _service.GetAllAsync(
            CancellationToken.None);


        // Assert
        var products = result.ToList();

        Assert.Equal(
            2,
            products.Count);


        Assert.Contains(
            products,
            p => p.Code == "PHONE-001");


        Assert.Contains(
            products,
            p => p.Code == "LAPTOP-001");


        Assert.All(
            products,
            p => Assert.Equal(
                "Electronics",
                p.CategoryName));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct_WhenProductExists()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronics"
        };

        var newCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Computers"
        };

        var product = new Product(
            "OLD-CODE",
            "Old Name",
            "Old Description",
            category.Id);


        _database.Context.Categories.AddRange(
            category,
            newCategory);

        _database.Context.Products.Add(product);

        await _database.Context.SaveChangesAsync();


        var request = new UpdateProductRequest
        {
            Code = "NEW-CODE",
            Name = "New Name",
            Description = "New Description",
            CategoryId = newCategory.Id
        };


        // Act
        await _service.UpdateAsync(
            product.Id,
            request,
            CancellationToken.None);


        // Assert
        var updatedProduct = await _database.Context.Products
            .FirstAsync(
                p => p.Id == product.Id);


        Assert.Equal(
            "NEW-CODE",
            updatedProduct.Code);

        Assert.Equal(
            "New Name",
            updatedProduct.Name);

        Assert.Equal(
            "New Description",
            updatedProduct.Description);

        Assert.Equal(
            newCategory.Id,
            updatedProduct.CategoryId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        var request = new UpdateProductRequest
        {
            Code = "NEW-CODE",
            Name = "New Name",
            Description = "New Description",
            CategoryId = Guid.NewGuid()
        };


        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () =>
            {
                await _service.UpdateAsync(
                    Guid.NewGuid(),
                    request,
                    CancellationToken.None);
            });


        Assert.Equal(
            "Product not found.",
            exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronics"
        };

        var product = new Product(
            "PHONE-001",
            "iPhone",
            "Mobile phone",
            category.Id);


        _database.Context.Categories.Add(category);

        _database.Context.Products.Add(product);

        await _database.Context.SaveChangesAsync();


        var request = new UpdateProductRequest
        {
            Code = "PHONE-UPDATED",
            Name = "Updated Phone",
            Description = "Updated description",
            CategoryId = Guid.NewGuid()
        };


        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
            {
                await _service.UpdateAsync(
                    product.Id,
                    request,
                    CancellationToken.None);
            });


        Assert.Equal(
            "Category does not exist.",
            exception.Message);


        var productInDb = await _database.Context.Products
            .FirstAsync(
                p => p.Id == product.Id);


        Assert.Equal(
            "PHONE-001",
            productInDb.Code);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteProduct_WhenProductExists()
    {
        // Arrange
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronics"
        };

        var product = new Product(
            "PHONE-001",
            "iPhone",
            "Mobile phone",
            category.Id);


        _database.Context.Categories.Add(category);
        _database.Context.Products.Add(product);

        await _database.Context.SaveChangesAsync();


        // Act
        await _service.DeleteAsync(
            product.Id,
            CancellationToken.None);


        // Assert
        var productInDb = await _database.Context.Products
            .IgnoreQueryFilters()
            .FirstAsync(
                p => p.Id == product.Id);


        Assert.NotNull(productInDb);

        Assert.False(
            productInDb.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();


        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () =>
            {
                await _service.DeleteAsync(
                    nonExistingId,
                    CancellationToken.None);
            });


        Assert.Equal(
            "Product not found.",
            exception.Message);
    }

    public void Dispose()
    {
        _database.Dispose();
    }
}