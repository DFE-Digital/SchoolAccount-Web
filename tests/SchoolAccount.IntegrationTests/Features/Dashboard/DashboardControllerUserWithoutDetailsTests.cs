using System.Net;
using SchoolAccount.IntegrationTests.Common.Scenarios;
using SchoolAccount.TestCommon.Builders;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Dashboard;

/// <summary>
/// Dashboard tests for a user with no id, name or email. They use their own scenario host,
/// so they live in their own class.
/// </summary>
public class DashboardControllerUserWithoutDetailsTests(UserWithoutDetailsHost host)
    : IClassFixture<UserWithoutDetailsHost>
{
    private readonly HttpClient _client = host.CreateTestClient();

    [Fact]
    public async Task A_user_with_no_id_email_or_organisation_is_rejected()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        var pageUri = host.GeneratePath("Dashboard", "Dashboard");

        host.CensusStatuses.Returns(
            CensusStatusesResponseBuilder.ACensusStatusResponse().AsSuccess()
        );

        // Act
        var message = await _client.GetAsync(pageUri, token);

        // Assert
        message.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }
}
