using System.Security.Claims;

namespace SchoolAccount.IntegrationTests.Common;

public class MockAuthClaimsOptions
{
    public IReadOnlyList<Claim>? Claims { get; init; }
}
