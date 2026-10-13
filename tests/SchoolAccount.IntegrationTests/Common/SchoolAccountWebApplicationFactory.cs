using JetBrains.Annotations;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Collect.CensusStatuses;
using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.SharedKernel;
using SchoolAccount.TestCommon.Fakes;

namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// Builds one host per authentication mode, once per test class. Tests change behaviour through
/// <see cref="ResetTestDoubles"/> rather than by registering services, so every client in a test
/// class talks to the same host and sees the same doubles.
/// </summary>
public class SchoolAccountWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    private readonly Lazy<WebApplicationFactory<TProgram>> _authorised;
    private readonly Lazy<WebApplicationFactory<TProgram>> _unauthenticated;
    private TestDoubles _testDoubles = new();

    public SchoolAccountWebApplicationFactory()
    {
        _authorised = new Lazy<WebApplicationFactory<TProgram>>(
            () => WithAuthenticationHandler<MockAuthHandler>()
        );
        _unauthenticated = new Lazy<WebApplicationFactory<TProgram>>(
            () => WithAuthenticationHandler<MockOidcHandler>()
        );
    }

    /// <summary>
    /// Replaces the doubles with fresh defaults and returns them. Call once from each test
    /// class constructor, so state set by one test never leaks into the next.
    /// </summary>
    public TestDoubles ResetTestDoubles() => _testDoubles = new TestDoubles();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
        builder.ConfigureTestServices(services =>
        {
            services.Replace(ServiceDescriptor.Singleton<IAntiforgery, DisabledAntiforgery>());

            // Resolved per scope from the current doubles, so a reset is seen by the next request.
            services.RemoveAll<IQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>>>();
            services.AddScoped<IQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>>>(
                _ => _testDoubles.CensusStatuses
            );
            services.RemoveAll<IQueryHandler<GetCensusJourneyQuery, GetCensusJourneyResponse>>();
            services.AddScoped<IQueryHandler<GetCensusJourneyQuery, GetCensusJourneyResponse>>(
                _ => _testDoubles.CensusJourney
            );
            services.RemoveAll<ICommandHandler<FeedbackCommand>>();
            services.AddScoped(_ => _testDoubles.Feedback);
            services.RemoveAll<IDateTimeProvider>();
            services.AddScoped<IDateTimeProvider>(_ => _testDoubles.DateTimeProvider);
            services.AddTransient(_ => new MockAuthClaimsOptions { Claims = _testDoubles.Claims });
        });
    }

    public HttpClient CreateAuthorisedClient(ClientOptions? options = null)
    {
        return CreateClient(_authorised.Value, options);
    }

    public HttpClient CreateUnauthenticatedClient(ClientOptions? options = null)
    {
        return CreateClient(_unauthenticated.Value, options);
    }

    public string GeneratePath(
        [AspMvcController] string controller,
        [AspMvcAction] string action,
        [AspMvcModelType] object? query = null
    )
    {
        using var scope = _authorised.Value.Services.CreateScope();
        var generator = scope.ServiceProvider.GetRequiredService<LinkGenerator>();
        return generator.GetPathByAction(action, controller.Replace("Controller", ""), query);
    }

    private static HttpClient CreateClient(
        WebApplicationFactory<TProgram> factory,
        ClientOptions? options
    )
    {
        options ??= new ClientOptions();

        return factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = options.AllowAutoRedirect }
        );
    }

    // Derived factories are tracked and disposed by this factory when the class fixture ends.
    private WebApplicationFactory<TProgram> WithAuthenticationHandler<THandler>()
        where THandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        const string schemeName = CookieAuthenticationDefaults.AuthenticationScheme;

        return WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
            {
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
            })
        );
    }
}
