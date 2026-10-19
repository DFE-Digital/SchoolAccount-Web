using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SchoolAccount.IntegrationTests.Common.Extensions;
using SchoolAccount.SharedKernel;
using SchoolAccount.TestCommon.Fakes;
using SchoolAccount.TestCommon.Stubs;

namespace SchoolAccount.IntegrationTests.Common.Scenarios;

/// <summary>
/// Builds the app once per test class for one scenario: who is signed in and what time it is.
/// Each scenario is a subclass, configured when the host is built and never changed. Tests
/// still set what the stubbed handlers return, before every request.
/// </summary>
public abstract class ScenarioHost : WebApplicationFactory<Program>
{
    public StubCensusStatusesHandler CensusStatuses { get; } = new();

    public StubCensusJourneyHandler CensusJourney { get; } = new();

    /// <summary>The signed-in user's claims, or null for the default school user.</summary>
    protected virtual IReadOnlyList<Claim>? Claims => null;

    /// <summary>A fixed time for the app's clock, or null for the real time.</summary>
    protected virtual DateTime? UtcNow => null;

    public HttpClient CreateTestClient() =>
        CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    public string GeneratePath(string controller, string action, object? query = null)
    {
        using var scope = Services.CreateScope();
        var generator = scope.ServiceProvider.GetRequiredService<LinkGenerator>();
        return generator.GetPathByAction(action, controller, query)!;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
        builder.ConfigureTestServices(services =>
        {
            TestAuthentication.Use<MockAuthHandler>(services);
            services.Replace(ServiceDescriptor.Singleton<IAntiforgery, DisabledAntiforgery>());
            services.AddSingleton(new MockAuthClaimsOptions { Claims = Claims });

            if (UtcNow is { } utcNow)
            {
                services.Replace(
                    ServiceDescriptor.Singleton<IDateTimeProvider>(new FixedClock(utcNow))
                );
            }

            services.StubQueryHandler(CensusStatuses);
            services.StubQueryHandler(CensusJourney);
        });
    }

    private sealed record FixedClock(DateTime UtcNow) : IDateTimeProvider;
}
