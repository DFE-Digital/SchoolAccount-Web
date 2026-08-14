namespace SchoolAccount.IntegrationTests.Common;

public class ClientOptions
{
    public bool AllowAutoRedirect { get; init; }

    public static readonly ClientOptions AllowRedirects = new() { AllowAutoRedirect = true };
}
