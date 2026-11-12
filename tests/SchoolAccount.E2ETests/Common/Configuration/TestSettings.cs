namespace SchoolAccount.E2ETests.Common.Configuration;

public sealed class TestSettings
{
    public bool Headless { get; init; } = true;

    public CaptureSettings Screenshot { get; init; } = new();

    public CaptureSettings Video { get; init; } = new();

    public DatabaseSettings? Database { get; init; }

    public AppAccessSettings SchoolAccount { get; init; } = new();

    /// <summary>
    /// The site under test, without a trailing slash. Comes from <c>SchoolAccount:Url</c>: user
    /// secrets first, then environment variables, then appsettings.json.
    /// </summary>
    public string SiteUrl => SchoolAccount.Url.TrimEnd('/');
}
