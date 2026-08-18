using SchoolAccount.Web.Mvc.Authentication;
using SchoolAccount.Web.Mvc.UnitTests.Helpers;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Authentication;

public class UserContextTests
{
    [Theory]
    [InlineData(true, "John", "Jones", "john.jones@testing.world", "Jones School")]
    [InlineData(
        true,
        "Lisa",
        "Simpson",
        "lisa.simpson@testing.world",
        "Springfield Elementary School"
    )]
    public void Ensure_that_an_authenticated_user_can_be_retrieved_from_the_user_context(
        bool isAuthenticated,
        string givenName,
        string familyName,
        string email,
        string organisationName
    )
    {
        // Arrange
        var accessor = HttpContextAccessorHelpers.CreateHttpContextAccessor(
            isAuthenticated,
            givenName,
            familyName,
            email,
            organisationName
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
        context.OrganisationName.ShouldNotBeNullOrEmpty();
        context.OrganisationName.ShouldContainWithoutWhitespace(organisationName);
    }

    [Fact]
    public void Ensure_that_an_unauthorised_request_occurs_when_user_context_is_flagged_as_unauthenticated()
    {
        // Arrange
        var accessor = HttpContextAccessorHelpers.CreateHttpContextAccessor(false);

        // Act
        var context = new UserContext(accessor);

        // Assert
        context.IsAuthenticated.ShouldBeFalse();
        context.GivenName.ShouldBeNullOrEmpty();
        context.Surname.ShouldBeNullOrEmpty();
        context.EmailAddress.ShouldBeNullOrEmpty();
        context.Name.ShouldBeNullOrWhiteSpace();
    }
}
