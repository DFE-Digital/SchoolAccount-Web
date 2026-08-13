using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
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
}
