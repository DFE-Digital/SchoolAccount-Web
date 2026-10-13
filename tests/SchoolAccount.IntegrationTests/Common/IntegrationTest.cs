namespace SchoolAccount.IntegrationTests.Common;

/// <summary>
/// Base for integration test classes. xUnit creates a new instance per test, so the doubles
/// are reset before every test while the host is shared across the whole class.
/// </summary>
public abstract class IntegrationTest(SchoolAccountWebApplicationFactory<Program> factory)
    : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    protected SchoolAccountWebApplicationFactory<Program> Factory { get; } = factory;

    protected TestDoubles TestDoubles { get; } = factory.ResetTestDoubles();
}
