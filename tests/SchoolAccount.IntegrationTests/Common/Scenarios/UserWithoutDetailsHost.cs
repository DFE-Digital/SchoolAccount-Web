using System.Security.Claims;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.IntegrationTests.Common.Scenarios;

/// <summary>A signed-in user with no id, name or email, and an organisation with only an id.</summary>
public sealed class UserWithoutDetailsHost : ScenarioHost
{
    protected override IReadOnlyList<Claim> Claims => [new(Organisation, """{"id": 12345}""")];
}
