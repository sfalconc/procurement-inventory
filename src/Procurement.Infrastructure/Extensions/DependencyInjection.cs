using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Procurement.Application.Common.Interfaces;
using Procurement.Application.Inventory.Interfaces;
using Procurement.Domain.Entities;

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

       services.AddIdentityCore<User>(options =>
       {
           options.Password.RequireDigit = true;
           options.Password.RequireLowercase = true;
           options.Password.RequireUppercase = true;
           options.Password.RequireNonAlphanumeric = true;
           options.Password.RequiredLength = 8;
           options.User.RequireUniqueEmail = false;
       })
       .AddRoles<IdentityRole<Guid>>()
       .AddSignInManager()
       .AddEntityFrameworkStores<ProcurementDbContext>();

       services.AddScoped<IApplicationDbContext, ProcurementDbContext>();
       services.AddScoped<IIdentityUserAuthenticator, IdentityUserAuthenticator>();
       services.AddScoped<IInventoryStockService, InventoryStockService>();
       services.AddScoped<INotificationService, NotificationService>();

       services.AddSingleton<IPasswordHasher, PasswordHasher>();
       services.AddScoped<IPasswordHasher<User>, IdentityPasswordHasher>();
       services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

       return services;
   }
}