using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Procurement.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(DependencyInjection).Assembly);
        });

        return services;
    }
}