namespace Procurement.Application.Common.Models;

public sealed record NotificationMessage(
    string Category,
    string Title,
    string Content,
    Guid? TargetId = null);
