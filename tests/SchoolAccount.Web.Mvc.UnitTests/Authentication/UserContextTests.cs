using SchoolAccount.Web.Mvc.Authentication;
using SchoolAccount.Web.Mvc.UnitTests.Helpers;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Authentication;

public class UserContextTests
{
    [Fact]
    public async Task Ensure_that_a_authenticated_user_can_be_retrieved_from_the_context_of_user_john()
    {
        // Arrange
        var accessor = HttpContextAccessorHelpers.CreateHttpContextAccessor(
            true,
            "John",
            "Jones",
            "john.jones@testing.world"
        );

        // Act
        var context = new UserContext(accessor);

        // Assert
        context.IsAuthenticated.ShouldBeTrue();
        context.GivenName.ShouldNotBeNullOrEmpty();
        context.GivenName.ShouldContainWithoutWhitespace("John");
        context.Surname.ShouldNotBeNullOrEmpty();
        context.Surname.ShouldContainWithoutWhitespace("Jones");
        context.EmailAddress.ShouldNotBeNullOrEmpty();
        context.EmailAddress.ShouldContainWithoutWhitespace("john.jones@testing.world");
        context.Name.ShouldNotBeNullOrWhiteSpace();
        context.Name.ShouldContainWithoutWhitespace("John Jones");
    }

    [Fact]
    public async Task Ensure_that_a_authenticated_user_can_be_retrieved_from_the_context_of_user_lisa()
    {
        // Arrange
        var accessor = HttpContextAccessorHelpers.CreateHttpContextAccessor(
            true,
            "Lisa",
            "Simpson",
            "lisa.simpson@testing.world"
        );

        // Act
        var context = new UserContext(accessor);

        // Assert
        context.IsAuthenticated.ShouldBeTrue();
        context.GivenName.ShouldNotBeNullOrEmpty();
        context.GivenName.ShouldContainWithoutWhitespace("Lisa");
        context.Surname.ShouldNotBeNullOrEmpty();
        context.Surname.ShouldContainWithoutWhitespace("Simpson");
        context.EmailAddress.ShouldNotBeNullOrEmpty();
        context.EmailAddress.ShouldContainWithoutWhitespace("lisa.simpson@testing.world");
        context.Name.ShouldNotBeNullOrWhiteSpace();
        context.Name.ShouldContainWithoutWhitespace("Lisa Simpson");
    }

    [Fact]
    public async Task When_a_unathorised_request_happens_user_context_is_flagged_as_unauthenticated()
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
