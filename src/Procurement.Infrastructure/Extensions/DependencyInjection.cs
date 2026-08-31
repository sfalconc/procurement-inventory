using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Procurement.Application.Common.Interfaces;
using Procurement.Application.Inventory.Interfaces;

using Procurement.Infrastructure.Persistence;
using Procurement.Infrastructure.Identity;
using Procurement.Infrastructure.Notifications;
using Procurement.Infrastructure.Services;

namespace Procurement.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ProcurementDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure()));
                
        services.AddScoped<IApplicationDbContext, ProcurementDbContext>();
        services.AddScoped<IInventoryStockService, InventoryStockService>();
        services.AddScoped<INotificationService, NotificationService>();
                
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}