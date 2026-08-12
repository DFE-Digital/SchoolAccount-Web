using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
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
        return CreateClient<MockAuthHandler>(additionalConfigurableServices, options);
    }

    public HttpClient CreateUnauthorisedClient(
        Action<IServiceCollection>? additionalConfigurableServices = null,
        ClientOptions? options = null
    )
    {
        return CreateClient<MockOidcHandler>(additionalConfigurableServices, options);
    }

    public string GeneratePath<T>(string action, object? query = null)
        where T : ControllerBase
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            return null;
        }

        var controllerType = typeof(T);
        return controllerType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Any(x => x.Name == action)
            ? GeneratePath(GetControllerRoute(controllerType), action, query)
            : throw new ArgumentException(
                $"Action {action} not found on controller {controllerType.Name}"
            );
    }

    public string GeneratePath(string controller, string action, object? query = null)
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
        const string schemeName = CookieAuthenticationDefaults.AuthenticationScheme;
        options ??= new ClientOptions();

        return WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IConfigureOptions<AuthenticationOptions>>();
                    services.RemoveAll<IAuthenticationSchemeProvider>();
                    services.RemoveAll<IConfigureOptions<OpenIdConnectOptions>>();
                    services.RemoveAll<IPostConfigureOptions<OpenIdConnectOptions>>();
                    services.RemoveAll<IConfigureOptions<CookieAuthenticationOptions>>();
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
                        .AddScheme<AuthenticationSchemeOptions, THandler>(schemeName, _ => { })
                        .AddScheme<AuthenticationSchemeOptions, THandler>(
                            OpenIdConnectDefaults.AuthenticationScheme,
                            _ => { }
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

    private static string GetControllerRoute(Type controllerType)
    {
        var template = controllerType.GetCustomAttribute<RouteAttribute>()?.Template;

        if (template?.Contains('{') == true && template?.Contains('}') == true)
        {
            template = null;
        }

        return template ?? controllerType.Name.Replace(nameof(Controller), string.Empty);
    }
}
