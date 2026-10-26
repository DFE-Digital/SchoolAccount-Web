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
    public async Task A_valid_submission_redirects_to_the_thank_you_message()
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
    public async Task An_invalid_submission_displays_errors_on_the_form()
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
            .TempData["ErrorSummary"]
            .ShouldBeOfType<Dictionary<string, string>>()
            .ShouldBe(
                new Dictionary<string, string>
                {
                    [nameof(FeedbackForm.FeedbackMessage)] =
                        "Enter your feedback or describe the problem",
                }
            );
        controller.TempData["FeedbackSubmitted"].ShouldBeNull();
        await _feedbackCommandHandler
            .DidNotReceive()
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Valid_feedback_is_successfully_submitted()
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
    public async Task Error_is_displayed_on_the_form_when_the_handler_fails()
    {
        // Arrange
        HandlerReturns(Result.Failure(SharedKernel.Error.Failure("test", "test")));
        using var controller = CreateController();

        // Act
        var result = await controller.Submit(Form("test-feedback"), _cancellationToken);

        // Assert
        result.ShouldBeOfType<LocalRedirectResult>().Url.ShouldBe("/Submit");
        controller
            .TempData["ErrorSummary"]
            .ShouldBeOfType<Dictionary<string, string>>()
            .ShouldBe(
                new Dictionary<string, string>
                {
                    [nameof(FeedbackForm.FeedbackMessage)] =
                        "Sorry, there is a problem sending your feedback. Try again later",
                }
            );
        controller.TempData["FeedbackSubmitted"].ShouldBeNull();
    }

    [Fact]
    public async Task Default_url_is_returned_with_external_return_url()
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
