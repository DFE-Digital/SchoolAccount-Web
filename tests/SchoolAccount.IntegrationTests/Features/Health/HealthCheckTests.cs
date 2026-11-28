using SchoolAccount.IntegrationTests.Common;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.TestCommon.Constants.UrlConstants;

namespace SchoolAccount.IntegrationTests.Features.Health;

public class HealthCheckTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_health_check_reports_healthy_without_signing_in()
    {
        // Arrange
        var client = factory.CreateUnauthenticatedClient();

        // Act
        var response = await client.GetAsync(HealthUrl, _cancellationToken);
        var body = await response.Content.ReadAsStringAsync(_cancellationToken);

        // Assert
        response.StatusCode.ShouldBe(OK);
        body.ShouldBe("Healthy");
    }
}
