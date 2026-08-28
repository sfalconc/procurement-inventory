namespace Procurement.Application.Common.Models;

public sealed record LowStockAlert(
    Guid ProductId,
    string ProductName,
    int CurrentQuantity,
    int MinimumStock);