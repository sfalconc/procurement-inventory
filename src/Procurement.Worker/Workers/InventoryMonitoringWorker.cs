using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;

namespace Procurement.Worker.Workers;

public class InventoryMonitoringWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<InventoryMonitoringWorker> _logger;
    private readonly INotificationService _notificationService;

    private readonly HashSet<Guid> _notifiedLowStockProducts = new();

    public InventoryMonitoringWorker(
        IServiceScopeFactory scopeFactory,
        INotificationService notificationService,
        ILogger<InventoryMonitoringWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _notificationService = notificationService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Inventory monitoring worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var dbContext =
                    scope.ServiceProvider
                        .GetRequiredService<IApplicationDbContext>();

                var lowStockItems =
                    await dbContext.Inventories
                        .AsNoTracking()
                        .Include(i => i.Product)
                        .Where(i =>
                            i.CurrentQuantity <= i.MinimumStock)
                        .Select(i => new
                        {
                            i.ProductId,
                            ProductName = i.Product.Name,
                            i.CurrentQuantity,
                            i.MinimumStock
                        })
                        .ToListAsync(stoppingToken);

                _logger.LogInformation(
                    "Inventory monitoring completed. Low stock items detected: {Count}",
                    lowStockItems.Count);

                foreach (var item in lowStockItems)
                {
                    _logger.LogWarning(
                        "Low stock detected for product {ProductId} ({ProductName}). Current quantity: {CurrentQuantity}, Minimum stock: {MinimumStock}",
                        item.ProductId,
                        item.ProductName,
                        item.CurrentQuantity,
                        item.MinimumStock);

                    if (_notifiedLowStockProducts.Add(item.ProductId))
                    {
                        await _notificationService.SendLowStockNotificationAsync(
                            item.ProductId,
                            item.ProductName,
                            item.CurrentQuantity,
                            item.MinimumStock,
                            stoppingToken);
                    }                 
                }

                var currentLowStockProductIds =
                    lowStockItems
                        .Select(item => item.ProductId)
                        .ToHashSet();

                _notifiedLowStockProducts
                    .RemoveWhere(id => !currentLowStockProductIds.Contains(id));               
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while monitoring inventory.");
            }

            await Task.Delay(
                TimeSpan.FromMinutes(5),
                stoppingToken);
        }

        _logger.LogInformation(
            "Inventory monitoring worker stopped.");
    }
}