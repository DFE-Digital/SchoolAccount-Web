using System.Security.Claims;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.IntegrationTests.Common.Scenarios;

/// <summary>
/// A user from a multi-academy trust with two schools, on 4 October 2026.
/// </summary>
public sealed class TrustUserHost : ScenarioHost
{
    public static readonly DateTime FourthOfOctober2026 = new(
        2026,
        10,
        4,
        9,
        0,
        0,
        DateTimeKind.Utc
    );

    private const string TrustJson = """
        {
         "id": "2E774B32-E4DB-445B-B915-736C777FF5A4",
         "name": "Test Multi Academy Trust",
         "category": { "id": "010", "name": "Multi Academy Trust" },
         "ukprn": "10037611",
         "establishments": [
             { "urn": "100001", "name": "Test School 1" },
             { "urn": "100002", "name": "Test School 2" }
         ]
        }
        """;

    protected override IReadOnlyList<Claim> Claims =>
        [
            new(GivenName, MockAuthHandler.FakeGivenName),
            new(FamilyName, MockAuthHandler.FakeFamilyName),
            new(Sub, "1159ee82-d515-4d34-b28d-ac138cb1506b"),
            new(Email, "test@example.com"),
            new(Organisation, TrustJson),
        ];

    protected override DateTime? UtcNow => FourthOfOctober2026;
}
