using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Collect.CensusStatuses;
using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.SharedKernel;
using SchoolAccount.TestCommon.Fakes;

namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// Builds the app once per test class, for a signed-in user, with every downstream dependency
/// replaced by a double from <see cref="TestDoubles"/>. Tests change the doubles rather than
/// registering services, so every client in a class talks to the same host.
/// </summary>
public sealed class SharedTestHost : WebApplicationFactory<Program>
{
    private TestDoubles _testDoubles = new();

    /// <summary>
    /// Replaces the doubles with fresh defaults and returns them. <see cref="SharedHostTest"/>
    /// calls this before every test, so state set by one test never leaks into the next.
    /// </summary>
    public TestDoubles ResetTestDoubles() => _testDoubles = new TestDoubles();

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

            // Resolved per request from the current doubles, so a reset is seen by the next request.
            services.Replace(
                ServiceDescriptor.Scoped<
                    IQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>>
                >(_ => _testDoubles.CensusStatuses)
            );
            services.Replace(
                ServiceDescriptor.Scoped<
                    IQueryHandler<GetCensusJourneyQuery, GetCensusJourneyResponse>
                >(_ => _testDoubles.CensusJourney)
            );
            services.Replace(
                ServiceDescriptor.Scoped<IDateTimeProvider>(_ => _testDoubles.DateTimeProvider)
            );
            services.AddTransient(_ => new MockAuthClaimsOptions { Claims = _testDoubles.Claims });
        });
    }
}
