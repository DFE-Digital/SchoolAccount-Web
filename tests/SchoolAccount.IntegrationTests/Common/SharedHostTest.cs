using Microsoft.AspNetCore.Mvc.Testing;

namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// Base for test classes that use <see cref="SharedTestHost"/>. xUnit creates a new instance per
/// test, so the doubles are reset before every test while the host is shared by the whole class.
/// </summary>
public abstract class SharedHostTest(SharedTestHost host) : IClassFixture<SharedTestHost>
{
    protected SharedTestHost Host { get; } = host;

    protected TestDoubles TestDoubles { get; } = host.ResetTestDoubles();

    protected HttpClient Client { get; } =
        host.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
}
