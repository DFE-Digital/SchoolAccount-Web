using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Features.Feedback;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Features.Feedback;

public class FeedbackControllerTests
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private static readonly ICommandHandler<FeedbackCommand> _feedbackCommandHandler =
        Substitute.For<ICommandHandler<FeedbackCommand>>();
    private static readonly IUserContext _userContext = Substitute.For<IUserContext>();

    [Fact]
    public async Task Submit_throws_when_message_is_empty()
    {
        // Arrange
        var message = string.Empty;
        using var controller = new FeedbackController(_userContext, _feedbackCommandHandler);

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await controller.Submit(message, null, _cancellationToken)
        );
    }

    [Fact]
    public async Task Submit_throws_when_message_exceeds_character_limit()
    {
        // Arrange
        var message = new string('a', 32001);
        using var controller = new FeedbackController(_userContext, _feedbackCommandHandler);

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await controller.Submit(message, null, _cancellationToken)
        );
    }

    [Fact]
    public async Task Submit_is_successful_when_message_is_32000_characters()
    {
        // Arrange
        var message = new string('a', 32000);
        _userContext.Organisation?.Id.Returns("test-id");
        _userContext.Organisation?.Ukprn.Returns("test-ukprn");
        _feedbackCommandHandler
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success);
        DefaultHttpContext httpContext = new();
        ActionContext actionContext = new(
            httpContext,
            new RouteData(),
            new ControllerActionDescriptor()
        );

        using FeedbackController controller = new(_userContext, _feedbackCommandHandler);
        controller.ControllerContext = new ControllerContext(actionContext);
        controller.TempData = new TempDataDictionary(
            httpContext,
            Substitute.For<ITempDataProvider>()
        );
        controller.Url = new UrlHelper(actionContext);

        // Act
        var result = await controller.Submit(message, "/Submit", _cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<LocalRedirectResult>();
        controller.TempData["FeedbackSubmitted"].ShouldBe(true);
    }

    [Fact]
    public async Task Submit_throws_when_handler_returns_failure()
    {
        // Arrange
        var message = new string('a', 32000);
        _userContext.Organisation?.Id.Returns("test-id");
        _userContext.Organisation?.Ukprn.Returns("test-ukprn");
        _feedbackCommandHandler
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>())
            .Returns(
                Result.Failure<RequestFailedException>(SharedKernel.Error.Failure("test", "test"))
            );
        DefaultHttpContext httpContext = new();
        ActionContext actionContext = new(
            httpContext,
            new RouteData(),
            new ControllerActionDescriptor()
        );

        using FeedbackController controller = new(_userContext, _feedbackCommandHandler);
        controller.ControllerContext = new ControllerContext(actionContext);
        controller.TempData = new TempDataDictionary(
            httpContext,
            Substitute.For<ITempDataProvider>()
        );
        controller.Url = new UrlHelper(actionContext);

        // Act
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await controller.Submit(message, "/Submit", _cancellationToken)
        );
    }
}
