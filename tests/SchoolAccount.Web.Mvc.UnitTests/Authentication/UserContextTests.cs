using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using SchoolAccount.Web.Mvc.Authentication;
using Shouldly;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.Web.Mvc.UnitTests.Authentication;

public class UserContextTests
{
    [Fact]
    public void Ensure_that_an_authenticated_user_can_be_retrieved_from_the_user_context_with_an_academy()
    {
        // Arrange
        var givenName = "John";
        var familyName = "Jones";
        var email = "john.jones@testing.world";
        var organisationJson =
            """{"id":"2E774B32-E4DB-445B-B915-736C777FF5A4","name":"East Herrington Primary Academy","category":{"id":"001","name":"Establishment"},"ukprn":"10037611","establishmentNumber":"2091","localAuthority":{"id":"502EF2E9-2CA6-4905-9BF7-E80695BD5717","name":"SUNDERLAND CITY METROPOLITAN BOROUGH COUNCIL","code":"394"}}""";

        var accessor = CreateHttpContextAccessor(
            true,
            givenName,
            familyName,
            email,
            organisationJson
        );

        // Act
        var context = new UserContext(accessor);

        // Assert
        context.IsAuthenticated.ShouldBeTrue();
        context.GivenName.ShouldNotBeNullOrEmpty();
        context.GivenName.ShouldContainWithoutWhitespace(givenName);
        context.Surname.ShouldNotBeNullOrEmpty();
        context.Surname.ShouldContainWithoutWhitespace(familyName);
        context.EmailAddress.ShouldNotBeNullOrEmpty();
        context.EmailAddress.ShouldContainWithoutWhitespace(email);
        context.Name.ShouldNotBeNullOrWhiteSpace();
        context.Name.ShouldContainWithoutWhitespace(givenName + " " + familyName);
        context.Organisation.ShouldNotBeNull();
        context.OrganisationName.ShouldContainWithoutWhitespace(context.Organisation.Name);
    }

    [Fact]
    public void Ensure_that_an_authenticated_user_can_be_retrieved_from_the_user_context_with_a_trust_school()
    {
        // Arrange
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

        var accessor = CreateHttpContextAccessor(
            true,
            givenName,
            familyName,
            email,
            organisationJson
        );

        // Act
        var context = new UserContext(accessor);

        // Assert
        context.IsAuthenticated.ShouldBeTrue();
        context.GivenName.ShouldNotBeNullOrEmpty();
        context.GivenName.ShouldContainWithoutWhitespace(givenName);
        context.Surname.ShouldNotBeNullOrEmpty();
        context.Surname.ShouldContainWithoutWhitespace(familyName);
        context.EmailAddress.ShouldNotBeNullOrEmpty();
        context.EmailAddress.ShouldContainWithoutWhitespace(email);
        context.Name.ShouldNotBeNullOrWhiteSpace();
        context.Name.ShouldContainWithoutWhitespace(givenName + " " + familyName);
        context.OrganisationName.ShouldContainWithoutWhitespace(context.Organisation.Name);
    }

    [Fact]
    public void Ensure_that_an_unauthorised_request_occurs_when_user_context_is_flagged_as_unauthenticated()
    {
        // Arrange
        var accessor = CreateHttpContextAccessor(false);

        // Act
        var context = new UserContext(accessor);

        // Assert
        context.IsAuthenticated.ShouldBeFalse();
        context.GivenName.ShouldBeNullOrEmpty();
        context.Surname.ShouldBeNullOrEmpty();
        context.EmailAddress.ShouldBeNullOrEmpty();
        context.Name.ShouldBeNullOrWhiteSpace();
    }

    private static IHttpContextAccessor CreateHttpContextAccessor(
        bool isAuthenticated,
        string? givenName = null,
        string? surname = null,
        string? email = null,
        string? organisation = null
    )
    {
        var claims = new List<Claim>();

        if (isAuthenticated)
        {
            if (givenName != null)
            {
                claims.Add(new Claim(GivenName, givenName));
            }

            if (surname != null)
            {
                claims.Add(new Claim(FamilyName, surname));
            }

            if (email != null)
            {
                claims.Add(new Claim(Email, email));
                claims.Add(new Claim(Id, Convert.ToBase64String(Encoding.UTF8.GetBytes(email))));
            }

            if (organisation != null)
            {
                claims.Add(new Claim(ClaimConstants.Organisation, organisation));
            }
        }

        var identity = new ClaimsIdentity(claims, isAuthenticated ? "test" : null);
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = principal };

        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(httpContext);

        return accessor;
    }
}
