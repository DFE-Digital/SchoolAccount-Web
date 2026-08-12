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
        Action<IServiceCollection>? additionalConfigurableServices = null,
        ClientOptions? options = null
    )
    {
        return CreateClient<MockAuthHandler>(
            MockAuthHandler.SchemeName,
            additionalConfigurableServices,
            options
        );
    }

    public HttpClient CreateUnauthorisedClient(
        Action<IServiceCollection>? additionalConfigurableServices = null,
        ClientOptions? options = null
    )
    {
        return CreateClient<MockOidcHandler>(
            MockOidcHandler.SchemeName,
            additionalConfigurableServices,
            options
        );
    }

    private HttpClient CreateClient<THandler>(
        string schemeName,
        Action<IServiceCollection>? additionalConfigurableServices = null,
        ClientOptions? options = null
    )
        where THandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        options ??= new ClientOptions();

        return WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IConfigureOptions<AuthenticationOptions>>();
                    services
                        .AddAuthentication(authenticationOptions =>
                        {
                            authenticationOptions.DefaultScheme = schemeName;
                            authenticationOptions.DefaultAuthenticateScheme = schemeName;
                            authenticationOptions.DefaultChallengeScheme = schemeName;
                            authenticationOptions.DefaultSignInScheme = schemeName;
                            authenticationOptions.DefaultSignOutScheme = schemeName;
                        })
                        .AddCookie(schemeName)
                        .AddScheme<AuthenticationSchemeOptions, THandler>(
                            OpenIdConnectDefaults.AuthenticationScheme,
                            configureOptions => { }
                        );

                    additionalConfigurableServices?.Invoke(services);
                })
            )
            .CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = options.AllowAutoRedirect,
                }
            );
    }
}
