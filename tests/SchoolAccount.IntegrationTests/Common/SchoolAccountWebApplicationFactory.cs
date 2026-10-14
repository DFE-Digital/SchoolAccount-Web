using JetBrains.Annotations;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ClearExtensions;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Collect.CensusStatuses;
using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.SharedKernel;
using SchoolAccount.TestCommon.Fakes;
using SchoolAccount.TestCommon.Stubs;
using static SchoolAccount.TestCommon.Builders.CensusStatusesResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyResponseBuilder;

namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// Builds the app once per test class, with every downstream dependency replaced by a test double
/// that has a sensible default. A page test runs without mocking anything; a test class or test
/// only overrides the doubles it cares about.
/// </summary>
/// <typeparam name="THandler">The authentication handler, which decides how the user is signed in.</typeparam>
public abstract class SchoolAccountWebApplicationFactory<THandler> : WebApplicationFactory<Program>
    where THandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public StubQueryHandler<
        GetCensusStatusesQuery,
        List<GetCensusStatusesResponse>
    > CensusStatuses { get; } = new();

    public StubQueryHandler<
        GetCensusJourneyQuery,
        GetCensusJourneyResponse
    > CensusJourney { get; } = new();

    public ICommandHandler<FeedbackCommand> Feedback { get; } =
        Substitute.For<ICommandHandler<FeedbackCommand>>();

    public FakeDateTimeProvider DateTimeProvider { get; } = new();

    public MockAuthClaimsOptions SignedInUser { get; } = new();

    /// <summary>
    /// Puts every test double back to its default and creates a client. Call it from the test
    /// class constructor, before overriding any doubles: calling it again resets them.
    /// </summary>
    public HttpClient CreateClientWithDefaults(ClientOptions? options = null)
    {
        ResetToDefaults();
        options ??= new ClientOptions();

        return CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = options.AllowAutoRedirect }
        );
    }

    public string GeneratePath(
        [AspMvcController] string controller,
        [AspMvcAction] string action,
        [AspMvcModelType] object? query = null
    )
    {
        using var scope = Services.CreateScope();
        var generator = scope.ServiceProvider.GetRequiredService<LinkGenerator>();
        return generator.GetPathByAction(action, controller.Replace("Controller", ""), query);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
        builder.ConfigureTestServices(services =>
        {
            ReplaceAuthentication(services);

            services.Replace(ServiceDescriptor.Singleton<IAntiforgery, DisabledAntiforgery>());
            services.Replace(ServiceDescriptor.Singleton<IDateTimeProvider>(DateTimeProvider));
            services.AddSingleton(SignedInUser);

            services.StubQueryHandler(CensusStatuses);
            services.StubQueryHandler(CensusJourney);
            services.AddScoped(_ => Feedback);
        });
    }

    private void ResetToDefaults()
    {
        CensusStatuses.Returns(ACensusStatusResponse().AsSuccess());
        CensusJourney.Returns(AGetCensusJourneyResponse().AsSuccess());

        Feedback.ClearSubstitute();
        Feedback
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        DateTimeProvider.UseRealClock();
        SignedInUser.Claims = null;
    }

    private static void ReplaceAuthentication(IServiceCollection services)
    {
        const string schemeName = CookieAuthenticationDefaults.AuthenticationScheme;

        services.RemoveAll<IConfigureOptions<AuthenticationOptions>>();
        services.RemoveAll<IAuthenticationSchemeProvider>();
        services.RemoveAll<IConfigureOptions<OpenIdConnectOptions>>();
        services.RemoveAll<IPostConfigureOptions<OpenIdConnectOptions>>();
        services.RemoveAll<IConfigureOptions<CookieAuthenticationOptions>>();

        services
            .AddAuthentication(authenticationOptions =>
            {
                authenticationOptions.DefaultScheme = schemeName;
                authenticationOptions.DefaultAuthenticateScheme = schemeName;
                authenticationOptions.DefaultChallengeScheme = schemeName;
                authenticationOptions.DefaultSignInScheme = schemeName;
                authenticationOptions.DefaultSignOutScheme = schemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, THandler>(schemeName, _ => { })
            .AddScheme<AuthenticationSchemeOptions, THandler>(
                OpenIdConnectDefaults.AuthenticationScheme,
                _ => { }
            );
    }
}

/// <summary>A signed-in user from a school. Override <c>SignedInUser</c> for anyone else.</summary>
public sealed class AuthorisedWebApplicationFactory
    : SchoolAccountWebApplicationFactory<MockAuthHandler>;

/// <summary>A visitor who hasn't signed in.</summary>
public sealed class UnauthenticatedWebApplicationFactory
    : SchoolAccountWebApplicationFactory<MockOidcHandler>;
