using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Procurement.Application.Products.Interfaces;
using Procurement.Application.Products.Services;

namespace Procurement.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        Console.WriteLine("### AddApplication ejecutado ###");
        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);

        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}