namespace SchoolAccount.E2ETests.Common.Configuration;

public sealed class DatabaseSettings
{
    public string Server { get; init; } = string.Empty;

    public string DatabaseName { get; init; } = string.Empty;

    public string UserId { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}
