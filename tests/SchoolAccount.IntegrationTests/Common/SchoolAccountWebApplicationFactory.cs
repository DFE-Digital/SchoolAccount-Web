using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace SchoolAccount.IntegrationTests.Common;

public class SchoolAccountWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
    }

    public HttpClient CreateAuthorisedClient()
    {
        return WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                    services
                        .AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = "TestScheme";
                            options.DefaultChallengeScheme = "TestScheme";
                        })
                        .AddScheme<AuthenticationSchemeOptions, MockAuthHandler>(
                            "TestScheme",
                            options => { }
                        )
                )
            )
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    public HttpClient CreateUnauthorisedClient()
    {
        return WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                    services
                        .AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = "TestScheme";
                            options.DefaultChallengeScheme = "TestScheme";
                        })
                        .AddScheme<AuthenticationSchemeOptions, MockOidcHandler>(
                            "TestScheme",
                            options => { }
                        )
                )
            )
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }
}
