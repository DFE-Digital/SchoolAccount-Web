using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FeatureManagement;
using NSubstitute;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.IntegrationTests.Common.Stubs;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Infrastructure;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.IntegrationTests.Common.Builders.Features.Collect.GetCensusJourney.Responses.GetCensusJourneyResponseBuilder;
using static SchoolAccount.TestCommon.Constants.UrlConstants;

namespace SchoolAccount.IntegrationTests.Features.Feedback;

public class FeedbackPageTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private readonly SchoolAccountWebApplicationFactory<Program> _factory;
    private readonly HttpClient _authenticatedClient;
    private readonly ICommandHandler<FeedbackCommand> _feedbackCommandHandler = Substitute.For<
        ICommandHandler<FeedbackCommand>
    >();
    private readonly CensusJourneyHandler _getCensusJourneyHandler = Stub.CensusJourneyHandler();

    public FeedbackPageTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _authenticatedClient = factory.CreateAuthorisedClient(services =>
        {
            services.AddScoped<ICommandHandler<FeedbackCommand>>(_ => _feedbackCommandHandler);
            services.StubQueryHandler(_getCensusJourneyHandler);
        });
    }

    [Fact]
    public async Task A_successful_feedback_submission_shows_the_thank_you_message()
    {
        // Arrange
        _feedbackCommandHandler
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        using var content = FeedbackForm("test-feedback");
        var response = await _authenticatedClient.PostAsync(
            FeedbackSubmissionUrl,
            content,
            _cancellationToken
        );

        // Assert
        response
            .Headers.Location.ShouldNotBeNull()
            .OriginalString.ShouldEndWith(FeedbackFooterThanksUrl);
    }

    [Fact]
    public async Task Empty_feedback_returns_the_user_to_the_form()
    {
        // Arrange & Act
        using var content = FeedbackForm(string.Empty);
        var response = await _authenticatedClient.PostAsync(
            FeedbackSubmissionUrl,
            content,
            _cancellationToken
        );

        // Assert
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(StartUrl);
        await _feedbackCommandHandler
            .DidNotReceive()
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(JourneyUrl)]
    [InlineData(FeedbackUrl)]
    public async Task The_page_shows_an_error_summary_and_message_when_the_message_is_empty(
        string returnUrl
    )
    {
        // Arrange
        _getCensusJourneyHandler
            .Handle(Arg.Any<GetCensusJourneyQuery>(), Arg.Any<CancellationToken>())
            .Returns(AGetCensusJourneyResponse().AsSuccess());

        // Known exception to one client per class: redirects are a client option, and each
        // client gets its own app host.
        var client = _factory.CreateAuthorisedClient(
            services => services.StubQueryHandler(_getCensusJourneyHandler),
            ClientOptions.AllowRedirects
        );

        // Act
        using var formContent = FeedbackForm(string.Empty, returnUrl);
        var response = await client.PostAsync(
            FeedbackSubmissionUrl,
            formContent,
            _cancellationToken
        );
        var content = await response.Content.ReadAsStringAsync(_cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(OK);

        content.ShouldSatisfyAllConditions(
            html => html.ShouldContain("<title>Error: "),
            html => html.ShouldContain("govuk-error-summary"),
            html =>
                html.IndexOf("govuk-error-summary", StringComparison.Ordinal)
                    .ShouldBeInRange(
                        html.IndexOf("<main", StringComparison.Ordinal),
                        html.IndexOf("<h1", StringComparison.Ordinal)
                    ),
            html =>
                html.ShouldContain(
                    "<a href=\"#FeedbackMessage\">Enter your feedback or describe the problem</a>"
                ),
            html => html.ShouldContain("data-has-errors=\"true\""),
            html => html.ShouldContain("govuk-form-group--error"),
            html => html.ShouldContain("govuk-textarea--error"),
            html => html.ShouldContain("id=\"FeedbackMessage-error\""),
            html =>
                html.ShouldContain(
                    "aria-describedby=\"what-do-you-want-to-tell-us-hint FeedbackMessage-error\""
                )
        );
    }

    [Fact]
    public async Task The_feedback_section_is_shown_when_the_feature_flag_is_enabled()
    {
        // Arrange
        _getCensusJourneyHandler
            .Handle(Arg.Any<GetCensusJourneyQuery>(), Arg.Any<CancellationToken>())
            .Returns(AGetCensusJourneyResponse().AsSuccess());

        // Act
        var html = await _authenticatedClient.GetStringAsync(JourneyUrl, _cancellationToken);

        // Assert
        html.ShouldContain("id=\"page-feedback\"");
    }

    [Fact]
    public async Task The_feedback_section_is_hidden_when_the_feature_flag_is_disabled()
    {
        // Arrange
        _getCensusJourneyHandler
            .Handle(Arg.Any<GetCensusJourneyQuery>(), Arg.Any<CancellationToken>())
            .Returns(AGetCensusJourneyResponse().AsSuccess());

        // Known exception to one client per class: switching the feature flag off replaces a
        // service, so it needs its own app host.
        var client = _factory.CreateAuthorisedClient(services =>
        {
            services.StubQueryHandler(_getCensusJourneyHandler);
            services.DisableFeedbackFeature();
        });

        // Act
        var html = await client.GetStringAsync(JourneyUrl, _cancellationToken);

        // Assert
        html.ShouldNotContain("id=\"page-feedback\"");
    }

    [Fact]
    public async Task Submit_returns_not_found_when_the_feature_flag_is_disabled()
    {
        // Arrange
        // Known exception to one client per class: switching the feature flag off replaces a
        // service, so it needs its own app host.
        var client = _factory.CreateAuthorisedClient(services =>
        {
            services.AddScoped<ICommandHandler<FeedbackCommand>>(_ => _feedbackCommandHandler);
            services.DisableFeedbackFeature();
        });

        // Act
        using var content = FeedbackForm("test-feedback");
        var response = await client.PostAsync(FeedbackSubmissionUrl, content, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(NotFound);
        await _feedbackCommandHandler
            .DidNotReceive()
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Feedback_form_page_shows_the_not_found_page_when_the_feature_flag_is_disabled()
    {
        // Arrange
        // Known exception to one client per class: switching the feature flag off replaces a
        // service, so it needs its own app host.
        var client = _factory.CreateAuthorisedClient(services => services.DisableFeedbackFeature());

        // Act
        var response = await client.GetAsync(FeedbackUrl, _cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(NotFound);
    }

    [Fact]
    public async Task The_feedback_form_page_is_shown()
    {
        // Arrange
        _feedbackCommandHandler
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var response = await _authenticatedClient.GetAsync(FeedbackUrl, _cancellationToken);
        var html = await response.Content.ReadAsStringAsync(_cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(OK);
        html.ShouldContain("<title>Feedback");
        html.ShouldContain("id=\"non-js-feedback-form\"");
        html.ShouldNotContain("id=\"page-feedback\"");
    }

    [Fact]
    public async Task The_feedback_thanks_page_is_shown_on_a_successful_submission_from_the_feedback_form_page()
    {
        // Arrange
        _feedbackCommandHandler
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        using var content = FeedbackForm("test-feedback", FeedbackUrl);
        var response = await _authenticatedClient.PostAsync(
            FeedbackSubmissionUrl,
            content,
            _cancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(Redirect);
        response.Headers.Location.ShouldNotBeNull().OriginalString.ShouldBe(FeedbackPageThanksUrl);
    }

    [Fact]
    public async Task The_feedback_form_page_does_not_show_the_sign_out_link()
    {
        // Arrange
        _feedbackCommandHandler
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var response = await _authenticatedClient.GetAsync(FeedbackUrl, _cancellationToken);
        var html = await response.Content.ReadAsStringAsync(_cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(OK);
        html.ShouldContain("class=\"header-navigation__school\"");
        html.ShouldNotContain("Sign out");
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
