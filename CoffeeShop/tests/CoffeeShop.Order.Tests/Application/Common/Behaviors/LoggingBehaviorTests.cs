namespace Zzaia.CoffeeShop.Order.Tests.Application.Common.Behaviors;

using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Zzaia.CoffeeShop.Order.Application.Common.Behaviors;

/// <summary>
/// Unit tests for LoggingBehavior.
/// </summary>
public sealed class LoggingBehaviorTests
{
    private readonly Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>> loggerMock;
    private readonly LoggingBehavior<TestRequest, TestResponse> behavior;

    public LoggingBehaviorTests()
    {
        loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>>();
        behavior = new LoggingBehavior<TestRequest, TestResponse>(loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldLogRequestHandling_WhenRequestIsProcessed()
    {
        TestRequest request = new("test");
        TestResponse expectedResponse = new("success");
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(expectedResponse);
        TestResponse result = await behavior.Handle(request, next, CancellationToken.None);
        result.Should().Be(expectedResponse);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Handling TestRequest")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogExecutionTime_WhenRequestCompletes()
    {
        TestRequest request = new("test");
        TestResponse expectedResponse = new("success");
        RequestHandlerDelegate<TestResponse> next = async () =>
        {
            await Task.Delay(10);
            return expectedResponse;
        };
        TestResponse result = await behavior.Handle(request, next, CancellationToken.None);
        result.Should().Be(expectedResponse);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Handled TestRequest in") && o.ToString()!.Contains("ms")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionOccurs()
    {
        TestRequest request = new("test");
        Exception expectedException = new("Test error");
        RequestHandlerDelegate<TestResponse> next = () => throw expectedException;
        Func<Task> act = async () => await behavior.Handle(request, next, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Error handling TestRequest after") && o.ToString()!.Contains("ms")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldRethrowException_WhenExceptionOccurs()
    {
        TestRequest request = new("test");
        Exception expectedException = new("Test error");
        RequestHandlerDelegate<TestResponse> next = () => throw expectedException;
        Func<Task> act = async () => await behavior.Handle(request, next, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>().WithMessage("Test error");
    }

    public sealed record TestRequest(string Value);
    public sealed record TestResponse(string Message);
}
