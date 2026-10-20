using SchoolAccount.Web.Mvc.ComponentTests.Common;
using SchoolAccount.Web.Mvc.Features.Start;
using Shouldly;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Start;

/// <summary>
/// Tests for the start page (Start.cshtml), the first page a signed-out user sees.
/// </summary>
/// <remarks>
/// The page has no data of its own: its model is empty and the content is fixed. These tests
/// guard the parts that matter, such as the sign-in link going to the right place and the
/// benefit cards (a separate partial, see StartCardsTests) being included.
/// </remarks>
public class StartTests(ComponentRenderer renderer) : IClassFixture<ComponentRenderer>
{
    private const string ViewPath = "~/Features/Start/Start.cshtml";

    [Fact]
    public async Task Heading_invites_the_user_to_sign_in()
    {
        // Arrange
        var model = new StartViewModel();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        html.QuerySelector("h1")
            .ShouldNotBeNull()
            .TextContent.ShouldBe("Sign in to School Account");
    }

    [Fact]
    public async Task Start_button_links_to_sign_in()
    {
        // Arrange
        var model = new StartViewModel();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // The view uses asp-controller="Account" asp-action="Login". The renderer turns that
        // into a real URL using the app's routes.
        var startButton = html.QuerySelector("a.govuk-button--start").ShouldNotBeNull();
        startButton.GetAttribute("href").ShouldBe("/Account/Login");
        startButton.TextContent.Trim().ShouldBe("Sign in or create an account");
    }

    [Fact]
    public async Task Start_button_arrow_is_hidden_from_screen_readers()
    {
        // Arrange
        var model = new StartViewModel();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // The arrow is decoration. aria-hidden="true" stops screen readers announcing it.
        var arrow = html.QuerySelector("a.govuk-button--start svg").ShouldNotBeNull();
        arrow.GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact]
    public async Task User_is_told_they_need_a_dfe_sign_in_account()
    {
        // Arrange
        var model = new StartViewModel();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        html.Body.ShouldNotBeNull()
            .TextContent.ShouldContain("You'll need a DfE Sign-in account.");
    }

    [Fact]
    public async Task Start_page_includes_the_benefit_cards()
    {
        // Arrange
        var model = new StartViewModel();

        // Act
        using var html = await renderer.RenderAsync(ViewPath, model);

        // Assert
        // Only checks the cards were included. Their content is tested in StartCardsTests.
        html.QuerySelectorAll(".dfe-card").Length.ShouldBe(3);
    }
}
