using System.Security.Claims;

namespace SchoolAccount.IntegrationTests.Common;

public class MockAuthClaimsOptions
{
    /// <summary>The signed-in user's claims, or <c>null</c> for the default school user.</summary>
    public IReadOnlyList<Claim>? Claims { get; set; }
}
