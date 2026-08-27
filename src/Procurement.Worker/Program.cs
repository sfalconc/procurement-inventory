using Procurement.Application.Common.Interfaces;
using Procurement.Infrastructure.Extensions;
using Procurement.Worker.Services;
using Procurement.Worker.Workers;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ICurrentUserService, WorkerCurrentUserService>();

builder.Services.AddHostedService<InventoryMonitoringWorker>();

var host = builder.Build();

host.Run();