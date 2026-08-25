using System.Security.Claims;

namespace SchoolAccount.IntegrationTests.Common;

public class MockAuthClaimsOptions
{
    public List<Claim>? Claims { get; set; }
}
