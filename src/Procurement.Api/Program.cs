using Azure.Identity;

using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using Procurement.Api.Exceptions;
using Procurement.Api.Authentication;
using Procurement.Api.Middleware;
using Procurement.Api.Services;

using Procurement.Application.Common.Constants;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Extensions;
using Procurement.Domain.Entities;

using Procurement.Infrastructure.Extensions;
using Procurement.Infrastructure.Persistence;
using Procurement.Infrastructure.Persistence.Seed;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Procurement.Api")
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .CreateLogger();

builder.Host.UseSerilog();

if (builder.Environment.IsProduction() || builder.Environment.IsStaging())
{
    var keyVaultUri = builder.Configuration["KeyVaultUri"];

    if (!string.IsNullOrEmpty(keyVaultUri))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential());
    }
}

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService("Procurement.Api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter();
    });

builder.Services.AddControllers();

builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")!);

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings");

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

                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings["SecretKey"]!)),

                RoleClaimType = ClaimTypes.Role
            };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        PolicyConstants.RequireSuperUser,
        policy =>
        {
            policy.RequireRole(RoleConstants.SuperUser);
        });

    options.AddPolicy(
        PolicyConstants.RequirePurchaser,
        policy =>
        {
            policy.RequireRole(
                RoleConstants.Purchaser,
                RoleConstants.SuperUser);
        });

    options.AddPolicy(
        PolicyConstants.RequireWarehouse,
        policy =>
        {
            policy.RequireRole(
                RoleConstants.Warehouse,
                RoleConstants.SuperUser);
        });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token."
        });

    options.AddSecurityRequirement(
        document =>
            new OpenApiSecuritySchemeReference("Bearer", document) == null
                ? new OpenApiSecurityRequirement()
                : new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(
                        "Bearer",
                        document)] = new List<string>()
                });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

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

        var userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager =
            scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        await ProcurementDbContextSeed.SeedAsync(
            context,
            userManager,
            roleManager,
            app.Environment.EnvironmentName);
    }
}

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseSerilogRequestLogging();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

if (app.Environment.IsEnvironment("Testing"))
{
    app.MapGet(
        "/api/force-error",
        () => { throw new Exception("Secret Info"); });
}

app.Run();

public partial class Program
{
}