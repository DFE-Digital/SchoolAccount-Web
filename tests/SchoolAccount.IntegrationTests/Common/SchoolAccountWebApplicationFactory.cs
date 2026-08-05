using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace SchoolAccount.IntegrationTests.Common;

public class SchoolAccountWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
    }

    public HttpClient CreateAuthorisedClient(
        Action<IServiceCollection>? additionalConfigurableServices = null
    )
    {
        return CreateClient<MockAuthHandler>(MockAuthHandler.SchemeName, additionalConfigurableServices);
    }

    public HttpClient CreateUnauthorisedClient(
        Action<IServiceCollection>? additionalConfigurableServices = null
    )
    {
        return CreateClient<MockOidcHandler>(MockOidcHandler.SchemeName, additionalConfigurableServices);
    }

    private HttpClient CreateClient<THandler>(
        string schemeName,
        Action<IServiceCollection>? additionalConfigurableServices = null
    )
        where THandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        return WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IConfigureOptions<AuthenticationOptions>>();
                    services
                        .AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = schemeName;
                            options.DefaultChallengeScheme = schemeName;
                        })
                        .AddScheme<AuthenticationSchemeOptions, THandler>(
                            OpenIdConnectDefaults.AuthenticationScheme,
                            options => { }
                        )
                        .AddScheme<AuthenticationSchemeOptions, THandler>(
                            schemeName,
                            options => { }
                        );

                    additionalConfigurableServices?.Invoke(services);
                })
            )
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }
}
