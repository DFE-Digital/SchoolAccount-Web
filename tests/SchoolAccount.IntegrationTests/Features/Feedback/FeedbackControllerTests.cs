using System.Net;
using NSubstitute;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.SharedKernel;
using SchoolAccount.TestCommon.Stubs;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Feedback;

public class FeedbackControllerTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private readonly SchoolAccountWebApplicationFactory<Program> _factory;
    private readonly HttpClient _authenticatedClient;
    private readonly ICommandHandler<FeedbackCommand> _feedbackCommandHandler = Substitute.For<
        ICommandHandler<FeedbackCommand>
    >();

    public FeedbackControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _authenticatedClient = factory.CreateAuthorisedClient(services =>
            services.AddScoped<ICommandHandler<FeedbackCommand>>(_ => _feedbackCommandHandler)
        );
    }

    [Fact]
    public async Task Submit_redirects_to_the_thank_you_message_when_feedback_is_sent()
    {
        // Arrange
        var pageUri = _factory.GeneratePath("Feedback", "Submit");
        _feedbackCommandHandler
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>())
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
        await _feedbackCommandHandler
            .DidNotReceive()
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task The_page_shows_an_error_summary_and_message_when_the_message_is_empty()
    {
        // Arrange
        var client = _factory.CreateAuthorisedClient(
            services => services.StubQueryHandler(StubCensusStatusesHandler.Succeeding()),
            ClientOptions.AllowRedirects
        );

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
            "<a href=\"#feedbackMessage\">Enter your feedback or describe the problem</a>"
        );
        html.ShouldContain("data-has-errors=\"true\"");
        html.ShouldContain("govuk-form-group--error");
        html.ShouldContain("govuk-textarea--error");
        html.ShouldContain("id=\"feedbackMessage-error\"");
        html.ShouldContain(
            "aria-describedby=\"what-do-you-want-to-tell-us-hint feedbackMessage-error\""
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
