using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Procurement.Api.Authentication;
using Procurement.Api.Middleware;
using Procurement.Api.Services;

using Procurement.Application.Common.Constants;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Extensions;

using Procurement.Infrastructure;
using Procurement.Infrastructure.Persistence.Seed;
using Procurement.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();

builder.Services.AddControllers();

builder.Services.AddHealthChecks();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings.SecretKey)),

                RoleClaimType = ClaimTypes.Role
            };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        PolicyConstants.RequireAdmin,
        policy =>
        {
            policy.RequireRole(RoleConstants.Admin);
        });

    options.AddPolicy(
        PolicyConstants.RequirePurchaser,
        policy =>
        {
            policy.RequireRole(RoleConstants.Purchaser);
        });

    options.AddPolicy(
        PolicyConstants.RequireWarehouse,
        policy =>
        {
            policy.RequireRole(RoleConstants.Warehouse);
        });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider
            .GetRequiredService<ProcurementDbContext>();

        if (context.Database.IsSqlServer())
        {
            await context.Database.MigrateAsync();
        }
        else
        {
            await context.Database.EnsureCreatedAsync();
        }

        await ProcurementDbContextSeed.SeedAsync(context);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

public partial class Program
{
}