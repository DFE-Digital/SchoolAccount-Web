using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using NSubstitute;
using SchoolAccount.Web.Mvc.Authentication;
using Shouldly;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.Web.Mvc.UnitTests.Authentication;

public class UserContextTests
{
    private readonly FakeLogger<UserContext> _logger = new();

    [Fact]
    public void Ensure_that_an_authenticated_user_can_be_retrieved_from_the_user_context_with_an_academy()
    {
        // Arrange
        var id = "EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE";
        var givenName = "John";
        var familyName = "Jones";
        var email = "john.jones@testing.world";
        var organisationJson = """
            {
             "id": "2E774B32-E4DB-445B-B915-736C777FF5A4",
             "name": "East Herrington Primary Academy",
             "category": {
                 "id": "001",
                 "name": "Establishment"
             },
             "ukprn": "10037611",
             "establishmentNumber": "2091",
             "localAuthority": {
                 "id": "502EF2E9-2CA6-4905-9BF7-E80695BD5717",
                 "name": "SUNDERLAND CITY METROPOLITAN BOROUGH COUNCIL",
                 "code": "394"
             }
            }
            """;

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
        context.IsAuthenticated.ShouldBeTrue();
        context.GivenName.ShouldBe(givenName);
        context.Surname.ShouldBe(familyName);
        context.Id.ShouldBe(id);
        context.EmailAddress.ShouldBe(email);
        context.Name.ShouldBe(givenName + " " + familyName);
        context.Organisation.ShouldNotBeNull();
        context.Organisation.Id.ShouldBe("2E774B32-E4DB-445B-B915-736C777FF5A4");
        context.Organisation.Name.ShouldBe("East Herrington Primary Academy");
        context.Organisation.Category.Id.ShouldBe("001");
        context.Organisation.Category.Name.ShouldBe("Establishment");
        context.Organisation.Ukprn.ShouldBe("10037611");
        context.Organisation.EstablishmentNumber?.ShouldBe("2091");
        context.Organisation.LocalAuthority.ShouldNotBeNull();
        context.Organisation.LocalAuthority.Id.ShouldBe("502EF2E9-2CA6-4905-9BF7-E80695BD5717");
        context.Organisation.LocalAuthority.Name.ShouldBe(
            "SUNDERLAND CITY METROPOLITAN BOROUGH COUNCIL"
        );
        context.Organisation.LocalAuthority.Code.ShouldBe("394");
    }

    [Fact]
    public void Ensure_that_an_authenticated_user_can_be_retrieved_from_the_user_context_with_a_trust_school()
    {
        // Arrange
        var id = "EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE";
        var givenName = "John";
        var familyName = "Jones";
        var email = "john.jones@testing.world";
        var organisationJson = """
             {
              "id": "DD9F7BD6-2828-4FE7-B6DA-7C7C028ED479",
              "name": "BALMORAL LEARNING TRUST",
              "category": {
                "id": "010",
                "name": "Multi-Academy Trust"
              },
              "ukprn": "10059806"
            }
            """;

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
        context.IsAuthenticated.ShouldBeTrue();
        context.GivenName.ShouldBe(givenName);
        context.Surname.ShouldBe(familyName);
        context.Id.ShouldBe(id);
        context.EmailAddress.ShouldBe(email);
        context.Name.ShouldBe(givenName + " " + familyName);
        context.Organisation.ShouldNotBeNull();
        context.Organisation.Id.ShouldBe("DD9F7BD6-2828-4FE7-B6DA-7C7C028ED479");
        context.Organisation.Name.ShouldBe("BALMORAL LEARNING TRUST");
        context.Organisation.Category.Id.ShouldBe("010");
        context.Organisation.Category.Name.ShouldBe("Multi-Academy Trust");
        context.Organisation.Ukprn.ShouldBe("10059806");
        context.Organisation.EstablishmentNumber.ShouldBeNull();
        context.Organisation.LocalAuthority.ShouldBeNull();
    }

    [Fact]
    public void Ensure_that_an_unauthorised_request_occurs_when_user_context_is_flagged_as_unauthenticated()
    {
        // Arrange
        var accessor = CreateHttpContextAccessor(false, null);

        // Act
        var context = new UserContext(accessor, _logger);

        // Assert
        context.IsAuthenticated.ShouldBeFalse();
        context.GivenName.ShouldBeNullOrEmpty();
        context.Surname.ShouldBeNullOrEmpty();
        context.EmailAddress.ShouldBeNullOrEmpty();
        context.Name.ShouldBeNullOrWhiteSpace();
        context.Organisation.ShouldBeNull();
    }

    [Fact]
    public void Ensure_that_malformed_json_is_handled_gracefully()
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
        _logger.Collector.LatestRecord.ShouldNotBeNull();
        _logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Warning);
        _logger.Collector.LatestRecord.Exception.ShouldBeOfType<JsonException>();
        _logger.Collector.LatestRecord.Message.ShouldContain(
            "Failed to deserialize organisation claim"
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
