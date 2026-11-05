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
    private readonly FakeLogger<UserContext> _logger = new();

    [Fact]
    public void A_signed_in_academy_users_details_are_read_from_their_claims()
    {
        // Arrange
        var id = "EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE";
        var givenName = "John";
        var familyName = "Jones";
        var email = "john.jones@testing.world";
        var organisationJson = OrganisationConstants.ValidAcademyJson;

        var claimsDictionary = new Dictionary<string, string>
        {
            [Sub] = id,
            [GivenName] = givenName,
            [FamilyName] = familyName,
            [Email] = email,
            [Organisation] = organisationJson,
        };

        var accessor = CreateHttpContextAccessor(true, claimsDictionary);

        // Act
        var context = new UserContext(accessor, _logger);

        // Assert
        var organisation = context.Organisation.ShouldNotBeNull();
        var localAuthority = organisation.LocalAuthority.ShouldNotBeNull();

        context.ShouldSatisfyAllConditions(
            user => user.IsAuthenticated.ShouldBeTrue(),
            user => user.GivenName.ShouldBe(givenName),
            user => user.Surname.ShouldBe(familyName),
            user => user.Id.ShouldBe(id),
            user => user.EmailAddress.ShouldBe(email),
            user => user.Name.ShouldBe(givenName + " " + familyName)
        );

        organisation.ShouldSatisfyAllConditions(
            org => org.Id.ShouldBe("2E774B32-E4DB-445B-B915-736C777FF5A4"),
            org => org.Name.ShouldBe("Test School"),
            org => org.Category.Id.ShouldBe("001"),
            org => org.Category.Name.ShouldBe("Establishment"),
            org => org.Ukprn.ShouldBe("10037611"),
            org => org.EstablishmentNumber.ShouldBe("2091")
        );

        localAuthority.ShouldSatisfyAllConditions(
            la => la.Id.ShouldBe("502EF2E9-2CA6-4905-9BF7-E80695BD5717"),
            la => la.Name.ShouldBe("SUNDERLAND CITY METROPOLITAN BOROUGH COUNCIL"),
            la => la.Code.ShouldBe("394")
        );
    }

    [Fact]
    public void A_signed_in_trust_users_details_are_read_from_their_claims()
    {
        // Arrange
        var id = "EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE";
        var givenName = "John";
        var familyName = "Jones";
        var email = "john.jones@testing.world";
        var organisationJson = OrganisationConstants.ValidTrustJson;

        var claimsDictionary = new Dictionary<string, string>
        {
            [Sub] = id,
            [GivenName] = givenName,
            [FamilyName] = familyName,
            [Email] = email,
            [Organisation] = organisationJson,
        };

        var accessor = CreateHttpContextAccessor(true, claimsDictionary);

        // Act
        var context = new UserContext(accessor, _logger);

        // Assert
        var organisation = context.Organisation.ShouldNotBeNull();

        context.ShouldSatisfyAllConditions(
            user => user.IsAuthenticated.ShouldBeTrue(),
            user => user.GivenName.ShouldBe(givenName),
            user => user.Surname.ShouldBe(familyName),
            user => user.Id.ShouldBe(id),
            user => user.EmailAddress.ShouldBe(email),
            user => user.Name.ShouldBe(givenName + " " + familyName)
        );

        organisation.ShouldSatisfyAllConditions(
            org => org.Id.ShouldBe("2E774B32-E4DB-445B-B915-736C777FF5A4"),
            org => org.Name.ShouldBe("Test Multi Academy Trust"),
            org => org.Category.Id.ShouldBe("010"),
            org => org.Category.Name.ShouldBe("Multi Academy Trust"),
            org => org.Ukprn.ShouldBe("10037611"),
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
        var id = "EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE";
        var givenName = "John";
        var familyName = "Jones";
        var email = "john.jones@testing.world";
        var organisationJson = "organisation is not valid json";

        var claimsDictionary = new Dictionary<string, string>
        {
            [Sub] = id,
            [GivenName] = givenName,
            [FamilyName] = familyName,
            [Email] = email,
            [Organisation] = organisationJson,
        };

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
}
