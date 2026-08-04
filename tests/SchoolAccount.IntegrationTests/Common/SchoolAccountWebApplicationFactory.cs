using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace SchoolAccount.IntegrationTests.Common;

public partial class SchoolAccountWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly bool _useAuthentication;
    private readonly bool _useNonAuthenticatedUser;

    public SchoolAccountWebApplicationFactory(Builder builder)
    {
        _useAuthentication = builder.UseAuthentication;
        _useNonAuthenticatedUser = builder.UseNonAuthenticatedUser;
    }

    public static Builder Create()
    {
        return new Builder();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
        builder.ConfigureTestServices(ConfigureTestServices);
    }

    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
        if (_useAuthentication)
        {
            services
                .AddAuthentication(SessionAuthenticationHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>(
                    SessionAuthenticationHandler.SchemeName,
                    _ => { }
                );
        }

        if (_useNonAuthenticatedUser)
        {
            services.AddTransient<IPolicyEvaluator, FakePolicyEvaluator>();
        }
    }
}
