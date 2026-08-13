using System.Net;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.Web.Mvc.Features.Accounts;
using SchoolAccount.Web.Mvc.Features.Start;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Account;

public class LoggedOutActionTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    [Fact]
    public async Task Ensure_an_authenticated_user_accessing_LoggedOut_are_redirected_to_the_start_page()
    {
        // Arrange
        var client = factory.CreateUnauthorisedClient();
        var requestUri = factory.GeneratePath("Account", "LoggedOut");

        // Act
        var response = await client.GetAsync(requestUri, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldStartWith(
            factory.GeneratePath("Start", "Start")
        );
    }

    [Fact]
    public async Task Ensure_LoggedOut_clears_a_users_session()
    {
        // Arrange
        var mockSession = Substitute.For<ISession>();
        var mockContextAssessor = Substitute.For<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext { Session = mockSession };

        mockContextAssessor.HttpContext.Returns(httpContext);

        var wasSessionCleared = false;
        var keys = new[] { "key1", "key2" };
        mockSession.Keys.Returns(keys);
        mockSession.When(s => s.Remove(Arg.Any<string>())).Do(_ => wasSessionCleared = true);

        var client = factory.CreateUnauthorisedClient(services =>
        {
            services.RemoveAll<IHttpContextAccessor>();
            services.AddSingleton(mockContextAssessor);
        });

        var requestUri = factory.GeneratePath("Account", "LoggedOut");

        // Act
        await client.GetAsync(requestUri, TestContext.Current.CancellationToken);

        // Assert
        wasSessionCleared.ShouldBeTrue();
    }
}
