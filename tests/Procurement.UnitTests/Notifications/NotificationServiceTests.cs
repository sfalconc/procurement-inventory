using Microsoft.Extensions.Logging;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Common.Models;
using Procurement.Infrastructure.Notifications;

namespace Procurement.UnitTests.Notifications;

public class NotificationServiceTests
{
    [Fact]
    public async Task NotificationService_ShouldImplementContractAndDeliverMessage()
    {
        var logger = new RecordingLogger<NotificationService>();
        INotificationService service = new NotificationService(logger);
        var message = new NotificationMessage(
            "LowStock",
            "Low stock",
            "Test notification",
            Guid.NewGuid());

        await service.SendAsync(message, CancellationToken.None);

        Assert.Contains("Test notification", Assert.Single(logger.Messages));
    }

    private sealed class RecordingLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }
    }
}
