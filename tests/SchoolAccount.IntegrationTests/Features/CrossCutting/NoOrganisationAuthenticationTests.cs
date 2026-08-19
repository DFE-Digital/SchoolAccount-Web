using System.Net;
using SchoolAccount.IntegrationTests.Common;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class NoOrganisationAuthenticationTests(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateNoOrganisationAuthorisedClient();

    [Theory]
    [InlineData("/")]
    [InlineData("Dashboard")]
    public async Task Users_with_no_organisation_error_with_status_code_forbidden(string path)
    {
        // Arrange & Act
        var response = await _client.GetAsync(path, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldNotBe(HttpStatusCode.Forbidden);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.Location?.OriginalString.ShouldBeEquivalentTo(
            factory.GeneratePath(
                "Error",
                "Error",
                new { HttpStatusCode = HttpStatusCode.Forbidden }
            )
        );
    }
}
