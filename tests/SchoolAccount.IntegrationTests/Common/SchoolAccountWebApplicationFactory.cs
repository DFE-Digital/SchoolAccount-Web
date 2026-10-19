using JetBrains.Annotations;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SchoolAccount.TestCommon.Fakes;

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
        return CreateClient<MockAuthHandler>(additionalConfigurableServices, options);
    }

    public HttpClient CreateUnauthenticatedClient(
        Action<IServiceCollection>? additionalConfigurableServices = null,
        ClientOptions? options = null
    )
    {
        return CreateClient<MockOidcHandler>(additionalConfigurableServices, options);
    }

    public string GeneratePath(
        [AspMvcController] string controller,
        [AspMvcAction] string action,
        [AspMvcModelType] object? query = null
    )
    {
        using var scope = Services.CreateScope();
        var generator = scope.ServiceProvider.GetRequiredService<LinkGenerator>();
        return generator.GetPathByAction(action, controller.Replace("Controller", ""), query);
    }

    private HttpClient CreateClient<THandler>(
        Action<IServiceCollection>? additionalConfigurableServices = null,
        ClientOptions? options = null
    )
        where THandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        options ??= new ClientOptions();

        return WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    TestAuthentication.Use<THandler>(services);
                    services.Replace(
                        ServiceDescriptor.Singleton<IAntiforgery, DisabledAntiforgery>()
                    );
                    services.TryAddSingleton(new MockAuthClaimsOptions());
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
