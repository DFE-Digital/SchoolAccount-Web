using SchoolAccount.IntegrationTests.Common;

[assembly: AssemblyFixture(typeof(HostCounter))]

namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// Counts how many app hosts the integration tests build. When the run finishes, the total is
/// written to hosts-built.txt next to the test assembly.
/// </summary>
public sealed class HostCounter : IDisposable
{
    private static int _hostsBuilt;

    public static void Increment() => Interlocked.Increment(ref _hostsBuilt);

    public void Dispose()
    {
        File.WriteAllText(
            Path.Combine(AppContext.BaseDirectory, "hosts-built.txt"),
            $"Hosts built: {_hostsBuilt}{Environment.NewLine}"
        );
    }
}
