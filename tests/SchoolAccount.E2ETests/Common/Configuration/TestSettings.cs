namespace SchoolAccount.E2ETests.Common.Configuration;

/// <summary>Every setting the suite reads. Each property below matches a section in appsettings.json.</summary>
public sealed class TestSettings
{
    public AspireSettings Aspire { get; init; } = new();

    public bool Headless { get; init; } = true;

    public CaptureSettings Screenshot { get; init; } = new();

    public CaptureSettings Video { get; init; } = new();

    public DatabaseSettings Database { get; init; } = new();

    public SchoolAccountSettings SchoolAccount { get; init; } = new();

    /// <summary>
    /// The site under test, without a trailing slash. Comes from <c>SchoolAccount:Url</c>: user
    /// secrets first, then environment variables, then appsettings.json.
    /// </summary>
    public string SiteUrl => SchoolAccount.Url.TrimEnd('/');
}

/// <summary>The site under test and the account the signed-in tests use.</summary>
public sealed class SchoolAccountSettings
{
    public string Url { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// The URN of the school to sign in as, when the account is linked to more than one
    /// organisation and DfE Sign-in asks which one to use.
    /// </summary>
    public string OrganisationUrn { get; init; } = string.Empty;
}

/// <summary>Whether the suite starts its own dependencies through the Aspire AppHost.</summary>
public sealed class AspireSettings
{
    /// <summary>
    /// When true, the run starts the AppHost and points the suite at it. Set it to false to run
    /// against a site and databases that are already running.
    /// </summary>
    public bool Enabled { get; init; } = true;
}

/// <summary>The SQL Server the database tests connect to.</summary>
public sealed class DatabaseSettings
{
    public string Server { get; init; } = string.Empty;

    public string UserId { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}

/// <summary>When to keep a screenshot or video of a test.</summary>
public sealed class CaptureSettings
{
    public bool Enabled { get; init; } = true;

    public bool CaptureOnSuccess { get; init; } = true;

    public bool CaptureOnFailure { get; init; } = true;

    public bool ShouldCapture(bool testFailed)
    {
        return Enabled && (testFailed ? CaptureOnFailure : CaptureOnSuccess);
    }
}
