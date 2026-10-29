using System.Net;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FeatureManagement;
using NSubstitute;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.SharedKernel;
using SchoolAccount.TestCommon.Stubs;
using SchoolAccount.Web.Mvc.Infrastructure;
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
        {
            services.AddScoped<ICommandHandler<FeedbackCommand>>(_ => _feedbackCommandHandler);
            services.StubQueryHandler(StubCensusJourneyHandler.Succeeding());
        });
    }

    [Fact]
    public async Task A_successful_feedback_submission_shows_the_thank_you_message()
    {
        // Arrange
        var pageUri = UrlConstants.FeedbackSubmissionUrl;
        _feedbackCommandHandler
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        using var content = FeedbackForm("test-feedback");
        var result = await _authenticatedClient.PostAsync(pageUri, content, _cancellationToken);

        // Assert
        result
            .Headers.Location.ShouldNotBeNull()
            .OriginalString.ShouldEndWith(UrlConstants.FeedbackFooterThanksUrl);
    }

    [Fact]
    public async Task Empty_feedback_returns_the_user_to_the_form()
    {
        // Arrange
        var pageUri = UrlConstants.FeedbackSubmissionUrl;
        using var content = FeedbackForm(string.Empty);

        // Act
        var result = await _authenticatedClient.PostAsync(pageUri, content, _cancellationToken);

        // Assert
        result.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(UrlConstants.BaseUrl);
        await _feedbackCommandHandler
            .DidNotReceive()
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(UrlConstants.JourneyUrl)]
    [InlineData(UrlConstants.FeedbackUrl)]
    public async Task The_page_shows_an_error_summary_and_message_when_the_message_is_empty(
        string returnUrl
    )
    {
        // Arrange
        var client = _factory.CreateAuthorisedClient(
            services => services.StubQueryHandler(StubCensusJourneyHandler.Succeeding()),
            ClientOptions.AllowRedirects
        );

        // Act
        using var content = FeedbackForm(string.Empty, returnUrl);
        var response = await client.PostAsync(
            UrlConstants.FeedbackSubmissionUrl,
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

    [Fact]
    public async Task The_feedback_section_is_shown_when_the_feature_flag_is_enabled()
    {
        // Arrange & Act
        var html = await _authenticatedClient.GetStringAsync(
            UrlConstants.JourneyUrl,
            _cancellationToken
        );

        // Assert
        html.ShouldContain("id=\"page-feedback\"");
    }

    [Fact]
    public async Task The_feedback_section_is_hidden_when_the_feature_flag_is_disabled()
    {
        // Arrange
        var client = _factory.CreateAuthorisedClient(services =>
        {
            services.StubQueryHandler(StubCensusJourneyHandler.Succeeding());
            services.DisableFeedbackFeature();
        });

        // Act
        var html = await client.GetStringAsync(UrlConstants.JourneyUrl, _cancellationToken);

        // Assert
        html.ShouldNotContain("id=\"page-feedback\"");
    }

    [Fact]
    public async Task Submit_returns_not_found_when_the_feature_flag_is_disabled()
    {
        // Arrange
        var client = _factory.CreateAuthorisedClient(services =>
        {
            services.AddScoped<ICommandHandler<FeedbackCommand>>(_ => _feedbackCommandHandler);
            services.DisableFeedbackFeature();
        });

        // Act
        using var content = FeedbackForm("test-feedback");
        var result = await client.PostAsync(
            UrlConstants.FeedbackSubmissionUrl,
            content,
            _cancellationToken
        );

        // Assert
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await _feedbackCommandHandler
            .DidNotReceive()
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Feedback_form_page_shows_the_not_found_page_when_the_feature_flag_is_disabled()
    {
        // Arrange
        var client = _factory.CreateAuthorisedClient(services => services.DisableFeedbackFeature());

        // Act
        var result = await client.GetAsync(UrlConstants.FeedbackUrl, _cancellationToken);

        // Assert
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task The_feedback_form_page_is_shown()
    {
        // Arrange
        var pageUri = UrlConstants.FeedbackUrl;
        _feedbackCommandHandler
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _authenticatedClient.GetAsync(pageUri, _cancellationToken);
        var html = await result.Content.ReadAsStringAsync(_cancellationToken);

        // Assert
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        html.ShouldContain("<title>Feedback");
        html.ShouldContain("id=\"non-js-feedback-form\"");
        html.ShouldNotContain("id=\"page-feedback\"");
    }

    [Fact]
    public async Task The_feedback_thanks_page_is_shown_on_a_successful_submission_from_the_feedback_form_page()
    {
        // Arrange
        var pageUri = UrlConstants.FeedbackSubmissionUrl;
        _feedbackCommandHandler
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        using var content = FeedbackForm("test-feedback", UrlConstants.FeedbackUrl);
        var result = await _authenticatedClient.PostAsync(pageUri, content, _cancellationToken);

        // Assert
        result.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        result
            .Headers.Location.ShouldNotBeNull()
            .OriginalString.ShouldBe(UrlConstants.FeedbackPageThanksUrl);
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

file static class FeedbackFeatureServiceCollectionExtensions
{
    public static void DisableFeedbackFeature(this IServiceCollection services)
    {
        var disabledFooter = new Dictionary<string, string?>
        {
            ["feature_management:feature_flags:0:id"] = FeatureFlags.FeedbackFooter,
            ["feature_management:feature_flags:0:enabled"] = "false",
        };

        var configurationRoot = new ConfigurationBuilder()
            .AddInMemoryCollection(disabledFooter)
            .Build();

        services.Replace(
            ServiceDescriptor.Singleton<IFeatureDefinitionProvider>(
                _ => new ConfigurationFeatureDefinitionProvider(configurationRoot)
            )
        );
    }
}
