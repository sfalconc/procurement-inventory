using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Procurement.Application.Common;
using Procurement.Application.Common.Interfaces;
using Procurement.Domain.Common;
using Procurement.Domain.Entities;
using Procurement.Infrastructure.DomainEvents;
using Procurement.Infrastructure.Persistence;
using Procurement.UnitTests.Common;

namespace Procurement.UnitTests.DomainEvents;

public class DomainEventInfrastructureTests
{
    [Fact]
    public void BaseEntity_ShouldCollectAndClearDomainEvents()
    {
        var entity = new TestEntity();
        var domainEvent = new TestDomainEvent();

        entity.AddDomainEvent(domainEvent);

        Assert.Single(entity.DomainEvents);
        Assert.Same(domainEvent, entity.DomainEvents.Single());

        entity.ClearDomainEvents();

        Assert.Empty(entity.DomainEvents);
    }

    [Fact]
    public async Task Dispatcher_ShouldPublishEventsThroughMediatRAndClearThem()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblyContaining<
                DomainEventInfrastructureTests>());
        services.AddTransient<
            INotificationHandler<DomainEventNotification>,
            TestDomainEventHandler>();
        await using var serviceProvider = services.BuildServiceProvider();

        var entity = new TestEntity();
        entity.AddDomainEvent(new TestDomainEvent());

        var dispatcher = new MediatRDomainEventDispatcher(
            serviceProvider.GetRequiredService<IPublisher>());

        await dispatcher.DispatchAsync(
            [entity],
            CancellationToken.None);

        Assert.True(TestDomainEventHandler.WasCalled);
        Assert.Empty(entity.DomainEvents);
        TestDomainEventHandler.WasCalled = false;
    }

    [Fact]
    public async Task DbContext_ShouldPersistChangesAndDispatchEvents()
    {
        using var database = new SqliteTestDatabase();
        var entity = new Category("Test", "Test category");
        entity.AddDomainEvent(new TestDomainEvent());

        var dispatcher = new RecordingDomainEventDispatcher();
        var options = new DbContextOptionsBuilder<ProcurementDbContext>()
            .UseSqlite(database.Context.Database.GetDbConnection())
            .Options;
        await using var context = new ProcurementDbContext(
            options,
            new TestCurrentUserService(),
            dispatcher);

        context.Add(entity);
        await context.SaveChangesAsync();

        Assert.Equal(1, await context.Categories.CountAsync());
        Assert.Single(dispatcher.Entities);
        Assert.Empty(entity.DomainEvents);
    }

    private sealed class TestEntity : BaseEntity
    {
    }

    private sealed class TestDomainEvent : IDomainEvent
    {
    }

    private sealed class TestDomainEventHandler
        : INotificationHandler<DomainEventNotification>
    {
        public static bool WasCalled { get; set; }

        public Task Handle(
            DomainEventNotification notification,
            CancellationToken cancellationToken)
        {
            WasCalled = notification.DomainEvent is TestDomainEvent;
            return Task.CompletedTask;
        }
    }

    private sealed class RecordingDomainEventDispatcher
        : IDomainEventDispatcher
    {
        public List<BaseEntity> Entities { get; } = [];

        public Task DispatchAsync(
            IReadOnlyCollection<BaseEntity> entities,
            CancellationToken cancellationToken)
        {
            Entities.AddRange(entities);

            foreach (BaseEntity entity in entities)
            {
                entity.ClearDomainEvents();
            }

            return Task.CompletedTask;
        }
    }
}
