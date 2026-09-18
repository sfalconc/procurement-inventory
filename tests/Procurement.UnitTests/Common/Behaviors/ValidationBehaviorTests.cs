using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Procurement.Application.Extensions;

namespace Procurement.UnitTests.Common.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Send_ShouldThrowValidationException_AndSkipHandler_WhenRequestIsInvalid()
    {
        TestHandler.Calls = 0;
        await using var provider = BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            mediator.Send(new TestRequest(string.Empty)));

        Assert.Contains(exception.Errors, error =>
            error.PropertyName == nameof(TestRequest.Value));
        Assert.Equal(0, TestHandler.Calls);
    }

    [Fact]
    public async Task Send_ShouldReachHandler_WhenRequestIsValid()
    {
        TestHandler.Calls = 0;
        await using var provider = BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var response = await mediator.Send(new TestRequest("valid"));

        Assert.Equal("handled", response);
        Assert.Equal(1, TestHandler.Calls);
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddTransient<
            IRequestHandler<TestRequest, string>,
            TestHandler>();
        services.AddTransient<
            IValidator<TestRequest>,
            TestRequestValidator>();

        return services.BuildServiceProvider();
    }

    private sealed record TestRequest(string Value) : IRequest<string>;

    private sealed class TestRequestValidator
        : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(request => request.Value)
                .NotEmpty();
        }
    }

    private sealed class TestHandler : IRequestHandler<TestRequest, string>
    {
        public static int Calls { get; set; }

        public Task<string> Handle(
            TestRequest request,
            CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult("handled");
        }
    }
}
