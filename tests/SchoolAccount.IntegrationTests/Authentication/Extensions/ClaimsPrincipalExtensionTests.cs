using System.Security.Claims;
using SchoolAccount.Web.Mvc.Authentication;
using SchoolAccount.Web.Mvc.Authentication.Extensions;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Authentication.Extensions;

public class ClaimsPrincipalExtensionsTests
{
    private static ClaimsPrincipal PrincipalWith(string? claimValue)
    {
        var claims = claimValue is null
            ? []
            : new[] { new Claim(ClaimConstants.Organisation, claimValue) };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
    }

    [Fact]
    public void Returns_organisation_when_claim_is_valid_json()
    {
        var principal = PrincipalWith("""{"name":"Test School"}""");

        var result = principal.GetOrganisation();

        result.ShouldNotBeNull();
        result.Name.ShouldBe("Test School");
    }

    [Fact]
    public void Is_case_insensitive_on_property_names()
    {
        var principal = PrincipalWith("""{"NAME":"Test School"}""");

        principal.GetOrganisation()?.Name.ShouldBe("Test School");
    }

    [Fact]
    public void Returns_null_when_claim_is_absent()
    {
        PrincipalWith(null).GetOrganisation().ShouldBeNull();
    }

    [Fact]
    public void Returns_null_when_claim_is_empty()
    {
        PrincipalWith("").GetOrganisation().ShouldBeNull();
    }
}
