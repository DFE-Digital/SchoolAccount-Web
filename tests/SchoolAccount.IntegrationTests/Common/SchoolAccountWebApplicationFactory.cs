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
        return WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IConfigureOptions<AuthenticationOptions>>();
                    services
                        .AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = MockAuthHandler.SchemeName;
                            options.DefaultChallengeScheme = MockAuthHandler.SchemeName;
                        })
                        .AddScheme<AuthenticationSchemeOptions, MockAuthHandler>(
                            MockAuthHandler.SchemeName,
                            options => { }
                        )
                        .AddScheme<AuthenticationSchemeOptions, MockAuthHandler>(
                            OpenIdConnectDefaults.AuthenticationScheme,
                            options => { }
                        );

                    additionalConfigurableServices?.Invoke(services);
                })
            )
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    public HttpClient CreateUnauthorisedClient(
        Action<IServiceCollection>? additionalConfigurableServices = null
    )
    {
        return WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IConfigureOptions<AuthenticationOptions>>();
                    services
                        .AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = MockOidcHandler.SchemeName;
                            options.DefaultChallengeScheme = MockOidcHandler.SchemeName;
                        })
                        .AddScheme<AuthenticationSchemeOptions, MockOidcHandler>(
                            OpenIdConnectDefaults.AuthenticationScheme,
                            options => { }
                        )
                        .AddScheme<AuthenticationSchemeOptions, MockOidcHandler>(
                            MockOidcHandler.SchemeName,
                            options => { }
                        );

                    additionalConfigurableServices?.Invoke(services);
                })
            )
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }
}
