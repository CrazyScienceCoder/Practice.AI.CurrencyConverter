using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Filters;

public partial class FluentValidationActionFilterSpecifications
{
    [Fact]
    public async Task OnActionExecutionAsync_NoActionArguments_InvokesNext()
    {
        var testBuilder = new TestBuilder();
        var filter = testBuilder.Build();
        var (context, nextCalled) = testBuilder.BuildContextWithArguments([]);

        await filter.OnActionExecutionAsync(context, nextCalled.Delegate);

        nextCalled.WasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task OnActionExecutionAsync_ArgumentIsNull_InvokesNext()
    {
        var testBuilder = new TestBuilder();
        var filter = testBuilder.Build();
        var (context, nextCalled) = testBuilder.BuildContextWithArguments(
            new Dictionary<string, object?> { ["arg"] = null });

        await filter.OnActionExecutionAsync(context, nextCalled.Delegate);

        nextCalled.WasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task OnActionExecutionAsync_ArgumentHasNoRegisteredValidator_InvokesNext()
    {
        var testBuilder = new TestBuilder()
            .WithNoValidatorRegistered();

        var filter = testBuilder.Build();
        var (context, nextCalled) = testBuilder.BuildContextWithArguments(
            new Dictionary<string, object?> { ["arg"] = new TestRequest { Name = "test" } });

        await filter.OnActionExecutionAsync(context, nextCalled.Delegate);

        nextCalled.WasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task OnActionExecutionAsync_ValidArgument_InvokesNext()
    {
        var testBuilder = new TestBuilder()
            .WithValidValidator();

        var filter = testBuilder.Build();
        var (context, nextCalled) = testBuilder.BuildContextWithArguments(
            new Dictionary<string, object?> { ["arg"] = new TestRequest { Name = "validName" } });

        await filter.OnActionExecutionAsync(context, nextCalled.Delegate);

        nextCalled.WasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task OnActionExecutionAsync_InvalidArgument_DoesNotInvokeNext()
    {
        var testBuilder = new TestBuilder()
            .WithInvalidValidator();

        var filter = testBuilder.Build();
        var (context, nextCalled) = testBuilder.BuildContextWithArguments(
            new Dictionary<string, object?> { ["arg"] = new TestRequest { Name = "" } });

        await filter.OnActionExecutionAsync(context, nextCalled.Delegate);

        nextCalled.WasCalled.Should().BeFalse();
    }

    [Fact]
    public async Task OnActionExecutionAsync_InvalidArgument_SetsBadRequestObjectResult()
    {
        var testBuilder = new TestBuilder()
            .WithInvalidValidator();

        var filter = testBuilder.Build();
        var (context, nextCalled) = testBuilder.BuildContextWithArguments(
            new Dictionary<string, object?> { ["arg"] = new TestRequest { Name = "" } });

        await filter.OnActionExecutionAsync(context, nextCalled.Delegate);

        context.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task OnActionExecutionAsync_InvalidArgument_ResultContainsValidationErrors()
    {
        var testBuilder = new TestBuilder()
            .WithInvalidValidator();

        var filter = testBuilder.Build();
        var (context, _) = testBuilder.BuildContextWithArguments(
            new Dictionary<string, object?> { ["arg"] = new TestRequest { Name = "" } });

        await filter.OnActionExecutionAsync(context, () => throw new InvalidOperationException("Should not be called"));

        var badRequest = (BadRequestObjectResult)context.Result!;
        badRequest.Value.Should().BeOfType<ValidationProblemDetails>();
    }

    [Fact]
    public async Task OnActionExecutionAsync_MultipleValidArguments_InvokesNext()
    {
        var testBuilder = new TestBuilder()
            .WithValidValidator();

        var filter = testBuilder.Build();
        var (context, nextCalled) = testBuilder.BuildContextWithArguments(
            new Dictionary<string, object?>
            {
                ["arg1"] = new TestRequest { Name = "valid1" },
                ["arg2"] = new TestRequest { Name = "valid2" }
            });

        await filter.OnActionExecutionAsync(context, nextCalled.Delegate);

        nextCalled.WasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task OnActionExecutionAsync_InvalidArgument_ResultStatusCodeIs400()
    {
        var testBuilder = new TestBuilder()
            .WithInvalidValidator();

        var filter = testBuilder.Build();
        var (context, _) = testBuilder.BuildContextWithArguments(
            new Dictionary<string, object?> { ["arg"] = new TestRequest { Name = "" } });

        await filter.OnActionExecutionAsync(context, () => throw new InvalidOperationException("Should not be called"));

        var badRequest = (BadRequestObjectResult)context.Result!;
        badRequest.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    internal sealed class TestRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    internal sealed class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }

    internal sealed class NextCalledTracker
    {
        public bool WasCalled { get; private set; }

        public ActionExecutionDelegate Delegate => async () =>
        {
            WasCalled = true;
            return await Task.FromResult<ActionExecutedContext>(null!);
        };
    }
}
