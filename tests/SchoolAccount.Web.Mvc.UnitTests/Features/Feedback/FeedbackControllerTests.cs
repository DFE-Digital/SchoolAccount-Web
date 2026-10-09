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
using SchoolAccount.SharedKernel.Authentication;
using SchoolAccount.Web.Mvc.Features.Feedback;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Features.Feedback;

public class FeedbackControllerTests
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private readonly ICommandHandler<FeedbackCommand> _feedbackCommandHandler = Substitute.For<
        ICommandHandler<FeedbackCommand>
    >();
    private readonly IUserContext _userContext = Substitute.For<IUserContext>();

    [Fact]
    public async Task Submit_redirects_to_the_thank_you_message_when_feedback_is_sent()
    {
        // Arrange
        HandlerReturns(Result.Success());
        using var controller = CreateController();

        // Act
        var result = await controller.Submit(Form("test-feedback"), _cancellationToken);

        // Assert
        result.ShouldBeOfType<LocalRedirectResult>().Url.ShouldBe("/Submit#feedback-thanks");
        controller.TempData["FeedbackSubmitted"].ShouldBe(true);
    }

    [Fact]
    public async Task Submit_redirects_back_to_the_form_with_its_errors_when_the_form_is_invalid()
    {
        // Arrange
        using var controller = CreateController();
        controller.ModelState.AddModelError(
            nameof(FeedbackForm.FeedbackMessage),
            "Enter your feedback or describe the problem"
        );

        // Act
        var result = await controller.Submit(Form(string.Empty), _cancellationToken);

        // Assert
        result.ShouldBeOfType<LocalRedirectResult>().Url.ShouldBe("/Submit");
        controller
            .TempData["FeedbackErrors"]
            .ShouldBeOfType<string[]>()
            .ShouldHaveSingleItem()
            .ShouldBe("Enter your feedback or describe the problem");
        controller.TempData["FeedbackSubmitted"].ShouldBeNull();
        await _feedbackCommandHandler
            .DidNotReceive()
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Submit_sends_the_message_and_organisation_to_the_handler()
    {
        // Arrange
        HandlerReturns(Result.Success());
        using var controller = CreateController();

        // Act
        await controller.Submit(Form("test-feedback"), _cancellationToken);

        // Assert
        await _feedbackCommandHandler
            .Received(1)
            .Handle(
                Arg.Is<FeedbackCommand>(command =>
                    command.Message == "test-feedback"
                    && command.Ukprn == "test-ukprn"
                    && command.OrganisationId == "test-id"
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Submit_throws_when_handler_returns_failure()
    {
        // Arrange
        HandlerReturns(Result.Failure(SharedKernel.Error.Failure("test", "test")));
        using var controller = CreateController();

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await controller.Submit(Form("test-feedback"), _cancellationToken)
        );
    }

    [Fact]
    public async Task Submit_redirects_to_root_when_return_url_is_not_local()
    {
        // Arrange
        HandlerReturns(Result.Success());
        using var controller = CreateController();

        // Act
        var result = await controller.Submit(
            Form("test-feedback", "https://www.google.com"),
            _cancellationToken
        );

        // Assert
        result.ShouldBeOfType<LocalRedirectResult>().Url.ShouldBe("/#feedback-thanks");
    }

    private void HandlerReturns(Result result) =>
        _feedbackCommandHandler
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>())
            .Returns(result);

    private static FeedbackForm Form(string message, string returnUrl = "/Submit") =>
        new() { FeedbackMessage = message, ReturnUrl = returnUrl };

    private FeedbackController CreateController()
    {
        _userContext.Organisation.Returns(
            new Organisation
            {
                Id = "test-id",
                Name = "test-organisation",
                Ukprn = "test-ukprn",
            }
        );

        DefaultHttpContext httpContext = new();
        ActionContext actionContext = new(
            httpContext,
            new RouteData(),
            new ControllerActionDescriptor()
        );

        return new FeedbackController(_userContext, _feedbackCommandHandler)
        {
            ControllerContext = new ControllerContext(actionContext),
            TempData = new TempDataDictionary(httpContext, Substitute.For<ITempDataProvider>()),
            Url = new UrlHelper(actionContext),
        };
    }
}
