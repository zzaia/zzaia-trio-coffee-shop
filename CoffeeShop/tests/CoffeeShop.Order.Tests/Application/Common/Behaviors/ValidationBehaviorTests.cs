namespace Zzaia.CoffeeShop.Order.Tests.Application.Common.Behaviors;

using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Zzaia.CoffeeShop.Order.Application.Common.Behaviors;

/// <summary>
/// Unit tests for ValidationBehavior.
/// </summary>
public sealed class ValidationBehaviorTests
{
    private readonly Mock<ILogger<ValidationBehavior<TestRequest, TestResponse>>> loggerMock;

    public ValidationBehaviorTests()
    {
        loggerMock = new Mock<ILogger<ValidationBehavior<TestRequest, TestResponse>>>();
    }

    [Fact]
    public async Task Handle_ShouldProceedToNextHandler_WhenNoValidatorsProvided()
    {
        List<IValidator<TestRequest>> validators = [];
        ValidationBehavior<TestRequest, TestResponse> behavior = new(validators, loggerMock.Object);
        TestRequest request = new("valid");
        TestResponse expectedResponse = new("success");
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(expectedResponse);
        TestResponse result = await behavior.Handle(request, next, CancellationToken.None);
        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Handle_ShouldProceedToNextHandler_WhenValidationPasses()
    {
        Mock<IValidator<TestRequest>> validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        List<IValidator<TestRequest>> validators = [validatorMock.Object];
        ValidationBehavior<TestRequest, TestResponse> behavior = new(validators, loggerMock.Object);
        TestRequest request = new("valid");
        TestResponse expectedResponse = new("success");
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(expectedResponse);
        TestResponse result = await behavior.Handle(request, next, CancellationToken.None);
        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
    {
        Mock<IValidator<TestRequest>> validatorMock = new Mock<IValidator<TestRequest>>();
        ValidationResult validationResult = new(new List<ValidationFailure>
        {
            new ValidationFailure("Value", "Value is required")
        });
        validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        List<IValidator<TestRequest>> validators = [validatorMock.Object];
        ValidationBehavior<TestRequest, TestResponse> behavior = new(validators, loggerMock.Object);
        TestRequest request = new("");
        TestResponse expectedResponse = new("success");
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(expectedResponse);
        Func<Task> act = async () => await behavior.Handle(request, next, CancellationToken.None);
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ShouldCollectAllValidationErrors_WhenMultipleValidatorsFail()
    {
        Mock<IValidator<TestRequest>> validator1Mock = new Mock<IValidator<TestRequest>>();
        ValidationResult validationResult1 = new(new List<ValidationFailure>
        {
            new ValidationFailure("Value", "First error")
        });
        validator1Mock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult1);
        Mock<IValidator<TestRequest>> validator2Mock = new Mock<IValidator<TestRequest>>();
        ValidationResult validationResult2 = new(new List<ValidationFailure>
        {
            new ValidationFailure("Value", "Second error")
        });
        validator2Mock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult2);
        List<IValidator<TestRequest>> validators = [validator1Mock.Object, validator2Mock.Object];
        ValidationBehavior<TestRequest, TestResponse> behavior = new(validators, loggerMock.Object);
        TestRequest request = new("");
        TestResponse expectedResponse = new("success");
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(expectedResponse);
        Func<Task> act = async () => await behavior.Handle(request, next, CancellationToken.None);
        await act.Should().ThrowAsync<ValidationException>().Where(ex => ex.Errors.Count() == 2);
    }

    [Fact]
    public async Task Handle_ShouldLogWarning_WhenValidationFails()
    {
        Mock<IValidator<TestRequest>> validatorMock = new Mock<IValidator<TestRequest>>();
        ValidationResult validationResult = new(new List<ValidationFailure>
        {
            new ValidationFailure("Value", "Value is required")
        });
        validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        List<IValidator<TestRequest>> validators = [validatorMock.Object];
        ValidationBehavior<TestRequest, TestResponse> behavior = new(validators, loggerMock.Object);
        TestRequest request = new("");
        TestResponse expectedResponse = new("success");
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(expectedResponse);
        try
        {
            await behavior.Handle(request, next, CancellationToken.None);
        }
        catch (ValidationException)
        {
        }
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Validation failed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    public sealed record TestRequest(string Value);
    public sealed record TestResponse(string Message);
}
