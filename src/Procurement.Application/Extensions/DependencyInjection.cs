using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Procurement.Application.Products.Interfaces;
using Procurement.Application.Products.Services;
using Procurement.Application.Suppliers.Interfaces;
using Procurement.Application.Suppliers.Services;

namespace Procurement.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);

        services.AddScoped<IProductService, ProductService>();

        services.AddScoped<ISupplierService, SupplierService>();

        return services;
    }
}