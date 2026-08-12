using System.Net;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.Web.Mvc;
using SchoolAccount.Web.Mvc.Features.Dashboard;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class AuthorisedAuthenticationTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateAuthorisedClient();

    public static IEnumerable<object[]> ProtectedRoutes
    {
        get
        {
            yield return
            [
                UrlBuilder.GeneratePath<DashboardController>(nameof(DashboardController.Dashboard)),
            ];
        }
    }

    [Theory]
    [MemberData(nameof(ProtectedRoutes))]
    public async Task Ensure_that_the_controller_redirects_for_authorised_users(string endpoint)
    {
        // Act
        var response = await _client.GetAsync(endpoint, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Found);
    }
}
