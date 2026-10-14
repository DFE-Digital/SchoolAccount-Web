using System.Net;
using NSubstitute;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.SharedKernel;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Feedback;

public class FeedbackControllerTests : IClassFixture<AuthorisedWebApplicationFactory>
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private readonly AuthorisedWebApplicationFactory _factory;
    private readonly HttpClient _authenticatedClient;

    public FeedbackControllerTests(AuthorisedWebApplicationFactory factory)
    {
        _factory = factory;
        _authenticatedClient = factory.CreateClientWithDefaults();
    }

    [Fact]
    public async Task Submit_redirects_to_the_thank_you_message_when_feedback_is_sent()
    {
        // Arrange
        var pageUri = _factory.GeneratePath("Feedback", "Submit");
        _factory
            .Feedback.Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        using var content = FeedbackForm("test-feedback");
        var result = await _authenticatedClient.PostAsync(pageUri, content, _cancellationToken);

        // Assert
        result.Headers.Location.ShouldNotBeNull().OriginalString.ShouldEndWith("#feedback-thanks");
    }

    [Fact]
    public async Task Submit_redirects_back_to_the_form_when_the_message_is_empty()
    {
        // Arrange
        var pageUri = _factory.GeneratePath("Feedback", "Submit");

        // Act
        using var content = FeedbackForm(string.Empty);
        var result = await _authenticatedClient.PostAsync(pageUri, content, _cancellationToken);

        // Assert
        result.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe("/");
        await _factory
            .Feedback.DidNotReceive()
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task The_page_shows_an_error_summary_and_message_when_the_message_is_empty()
    {
        // Arrange
        var client = _factory.CreateClientWithDefaults(ClientOptions.AllowRedirects);

        // Act
        using var content = FeedbackForm(
            string.Empty,
            _factory.GeneratePath("Dashboard", "Dashboard")
        );
        var response = await client.PostAsync(
            _factory.GeneratePath("Feedback", "Submit"),
            content,
            _cancellationToken
        );
        var html = await response.Content.ReadAsStringAsync(_cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        html.ShouldContain("<title>Error: ");
        html.ShouldContain("govuk-error-summary");
        html.IndexOf("govuk-error-summary", StringComparison.Ordinal)
            .ShouldBeInRange(
                html.IndexOf("<main", StringComparison.Ordinal),
                html.IndexOf("<h1", StringComparison.Ordinal)
            );
        html.ShouldContain(
            "<a href=\"#FeedbackMessage\">Enter your feedback or describe the problem</a>"
        );
        html.ShouldContain("data-has-errors=\"true\"");
        html.ShouldContain("govuk-form-group--error");
        html.ShouldContain("govuk-textarea--error");
        html.ShouldContain("id=\"FeedbackMessage-error\"");
        html.ShouldContain(
            "aria-describedby=\"what-do-you-want-to-tell-us-hint FeedbackMessage-error\""
        );
    }

    private static FormUrlEncodedContent FeedbackForm(string message, string? returnUrl = null)
    {
        var fields = new Dictionary<string, string> { ["feedbackMessage"] = message };

        if (returnUrl is not null)
        {
            fields["returnUrl"] = returnUrl;
        }

        return new FormUrlEncodedContent(fields);
    }
}
