using Microsoft.Playwright;

namespace SchoolAccount.EndToEndTests.Common;

/// <summary>
/// Test users. mock-oauth2-server accepts any username and any claims at
/// sign-in, so each user's claims live here rather than in the mock's config.
/// </summary>
internal static class TestUsers
{
    public const string AcademyA = "academy-a";
    public const string AcademyAName = "Test Academy A";

    public const string AcademyB = "academy-b";
    public const string AcademyBName = "Test Academy B";

    private static readonly Dictionary<string, string> _claims = new()
    {
        [AcademyA] = """
            {
              "given_name": "Alex",
              "family_name": "Tester",
              "email": "alex.tester@example.com",
              "organisation": {
                "id": "0B5D6A7E-2C1F-4E3A-9D8B-7A6C5B4E3D2F",
                "name": "Test Academy A",
                "category": { "id": "001", "name": "Establishment" },
                "ukprn": "10000001",
                "establishmentNumber": "4001",
                "localAuthority": {
                  "id": "502EF2E9-2CA6-4905-9BF7-E80695BD5717",
                  "name": "SUNDERLAND CITY METROPOLITAN BOROUGH COUNCIL",
                  "code": "394"
                }
              }
            }
            """,
        [AcademyB] = """
            {
              "given_name": "Sam",
              "family_name": "Tester",
              "email": "sam.tester@example.com",
              "organisation": {
                "id": "1C6E7B8F-3D2A-4F4B-8E9C-8B7D6C5F4E3A",
                "name": "Test Academy B",
                "category": { "id": "001", "name": "Establishment" },
                "ukprn": "10000002",
                "establishmentNumber": "4002",
                "localAuthority": {
                  "id": "502EF2E9-2CA6-4905-9BF7-E80695BD5717",
                  "name": "SUNDERLAND CITY METROPOLITAN BOROUGH COUNCIL",
                  "code": "394"
                }
              }
            }
            """,
    };

    public static string ClaimsFor(string username) => _claims[username];
}

internal static class MockSignIn
{
    /// <summary>
    /// Signs in through mock-oauth2-server's login page, sending the user's
    /// claims, and lands on the start page, which needs no backing APIs.
    /// </summary>
    public static async Task SignInAsAsync(IPage page, string username)
    {
        await page.GotoAsync("/account/login?returnUrl=%2F");
        await page.Locator("input[name=username]").FillAsync(username);
        await page.Locator("textarea[name=claims]").FillAsync(TestUsers.ClaimsFor(username));
        await page.Locator("input[type=submit]").ClickAsync();
        await page.WaitForURLAsync(url => new Uri(url).AbsolutePath == "/");
    }
}
