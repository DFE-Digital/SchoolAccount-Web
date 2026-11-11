using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using NSubstitute;
using SchoolAccount.TestCommon.Constants;
using SchoolAccount.Web.Mvc.Authentication;
using Shouldly;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.Web.Mvc.UnitTests.Authentication;

public class UserContextTests
{
    private const string _id = "EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE";
    private const string _givenName = "John";
    private const string _familyName = "Jones";
    private const string _email = "john.jones@testing.world";
    private readonly FakeLogger<UserContext> _logger = new();

    [Fact]
    public void A_signed_in_academy_users_details_are_read_from_their_claims()
    {
        // Arrange
        var organisationJson = OrganisationConstants.SingleSchoolJson;

        var claimsDictionary = CreateClaimsDictionary(organisationJson);

        var accessor = CreateHttpContextAccessor(true, claimsDictionary);

        // Act
        var context = new UserContext(accessor, _logger);

        // Assert
        var organisation = context.Organisation.ShouldNotBeNull();
        var localAuthority = organisation.LocalAuthority.ShouldNotBeNull();

        context.ShouldSatisfyAllConditions(
            user => user.IsAuthenticated.ShouldBeTrue(),
            user => user.GivenName.ShouldBe(_givenName),
            user => user.Surname.ShouldBe(_familyName),
            user => user.Id.ShouldBe(_id),
            user => user.EmailAddress.ShouldBe(_email),
            user => user.Name.ShouldBe(_givenName + " " + _familyName)
        );

        organisation.ShouldSatisfyAllConditions(
            org => org.Id.ShouldBe(OrganisationConstants.FakeSchoolId),
            org => org.Name.ShouldBe(OrganisationConstants.FakeSchoolName),
            org => org.Category.Id.ShouldBe(OrganisationConstants.FakeCategoryEstablishmentId),
            org => org.Category.Name.ShouldBe(OrganisationConstants.FakeCategoryEstablishmentName),
            org => org.Ukprn.ShouldBe(OrganisationConstants.FakeSchoolUkprn),
            org =>
                org.EstablishmentNumber.ShouldBe(
                    OrganisationConstants.FakeSchoolEstablishmentNumber
                )
        );

        localAuthority.ShouldSatisfyAllConditions(
            la => la.Id.ShouldBe(OrganisationConstants.FakeSchoolLocalAuthorityId),
            la => la.Name.ShouldBe(OrganisationConstants.FakeSchoolLocalAuthorityName),
            la => la.Code.ShouldBe(OrganisationConstants.FakeSchoolLocalAuthorityCode)
        );
    }

    [Fact]
    public void A_signed_in_trust_users_details_are_read_from_their_claims()
    {
        // Arrange
        var organisationJson = OrganisationConstants.TrustJson;

        var claimsDictionary = CreateClaimsDictionary(organisationJson);

        var accessor = CreateHttpContextAccessor(true, claimsDictionary);

        // Act
        var context = new UserContext(accessor, _logger);

        // Assert
        var organisation = context.Organisation.ShouldNotBeNull();

        context.ShouldSatisfyAllConditions(
            user => user.IsAuthenticated.ShouldBeTrue(),
            user => user.GivenName.ShouldBe(_givenName),
            user => user.Surname.ShouldBe(_familyName),
            user => user.Id.ShouldBe(_id),
            user => user.EmailAddress.ShouldBe(_email),
            user => user.Name.ShouldBe(_givenName + " " + _familyName)
        );

        organisation.ShouldSatisfyAllConditions(
            org => org.Id.ShouldBe(OrganisationConstants.FakeTrustId),
            org => org.Name.ShouldBe(OrganisationConstants.FakeTrustName),
            org => org.Category.Id.ShouldBe(OrganisationConstants.FakeCategoryTrustId),
            org => org.Category.Name.ShouldBe(OrganisationConstants.FakeCategoryTrustName),
            org => org.Ukprn.ShouldBe(OrganisationConstants.FakeTrustUkprn),
            org => org.EstablishmentNumber.ShouldBeNull(),
            org => org.LocalAuthority.ShouldBeNull()
        );
    }

    [Fact]
    public void A_signed_out_user_has_no_details()
    {
        // Arrange
        var accessor = CreateHttpContextAccessor(false, null);

        // Act
        var context = new UserContext(accessor, _logger);

        // Assert
        context.ShouldSatisfyAllConditions(
            user => user.IsAuthenticated.ShouldBeFalse(),
            user => user.GivenName.ShouldBeNullOrEmpty(),
            user => user.Surname.ShouldBeNullOrEmpty(),
            user => user.EmailAddress.ShouldBeNullOrEmpty(),
            user => user.Name.ShouldBeNullOrWhiteSpace(),
            user => user.Organisation.ShouldBeNull()
        );
    }

    [Fact]
    public void An_unreadable_organisation_claim_is_ignored_and_logged()
    {
        // Arrange
        var organisationJson = "organisation is not valid json";

        var claimsDictionary = CreateClaimsDictionary(organisationJson);

        var accessor = CreateHttpContextAccessor(true, claimsDictionary);

        // Act
        var context = new UserContext(accessor, _logger);

        // Assert
        context.Organisation.ShouldBeNull();
        _logger.ShouldSatisfyAllConditions(
            logger => logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Warning),
            logger => logger.Collector.LatestRecord.Exception.ShouldBeOfType<JsonException>(),
            logger =>
                logger.Collector.LatestRecord.Message.ShouldContain(
                    "Failed to deserialize organisation claim"
                )
        );
    }

    private static IHttpContextAccessor CreateHttpContextAccessor(
        bool isAuthenticated,
        Dictionary<string, string>? claimsDictionary
    )
    {
        var claims = new List<Claim>();

        if (isAuthenticated)
        {
            claims = claimsDictionary?.Select(kvp => new Claim(kvp.Key, kvp.Value)).ToList();
        }

        var identity = new ClaimsIdentity(claims, isAuthenticated ? "test" : null);
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = principal };

        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(httpContext);

        return accessor;
    }

    private static Dictionary<string, string> CreateClaimsDictionary(string organisationJson)
    {
        return new Dictionary<string, string>
        {
            [Sub] = _id,
            [GivenName] = _givenName,
            [FamilyName] = _familyName,
            [Email] = _email,
            [Organisation] = organisationJson,
        };
    }
}
