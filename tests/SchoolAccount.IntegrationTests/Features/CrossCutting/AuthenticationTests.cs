using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using SchoolAccount.IntegrationTests.Common;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.CrossCutting;

public class AuthenticationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public AuthenticationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
    }
    
    [Fact]
    public async Task Ensure_that_the_controller_redirects_for_unauthorised_users()
    {
        // Act
        var response = await _client.GetAsync("/dashboard", TestContext.Current.CancellationToken);
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.Location?.OriginalString.ShouldStartWith("https://test-oidc.signin");
    }
}
