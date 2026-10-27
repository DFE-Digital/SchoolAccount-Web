using AngleSharp.Dom;
using SchoolAccount.Web.Mvc.ComponentTests.Common;
using Shouldly;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Start;

/// <summary>
/// Tests for the "What you'll get" cards shown on the start page (_StartCards.cshtml).
/// </summary>
/// <remarks>
/// This partial has no model: its content is fixed. That makes it the simplest component in
/// the site, and a good one to read first if you're new to these tests.
/// </remarks>
public sealed class StartCardsTests(ComponentRenderer renderer) : IClassFixture<ComponentRenderer>
{
    private const string ViewPath = "~/Features/Start/_StartCards.cshtml";

    [Fact]
    public async Task Cards_are_introduced_with_a_heading()
    {
        // Act
        using var html = await RenderCardsAsync();

        // Assert
        html.QuerySelector("h2").ShouldNotBeNull().TextContent.Trim().ShouldBe("What you'll get");
    }

    [Fact]
    public async Task Each_card_describes_a_benefit_of_the_service()
    {
        // Act
        using var html = await RenderCardsAsync();

        // Assert
        html.QuerySelectorAll(".dfe-card p")
            .Select(text => text.TextContent.Trim())
            .ShouldBe([
                "Step-by-step guidance to help you complete your statutory returns",
                "Statuses that show where you are in the process",
                "Email notifications when something needs your attention",
            ]);
    }

    [Fact]
    public async Task Card_images_are_decorative_so_screen_readers_skip_them()
    {
        // Act
        using var html = await RenderCardsAsync();

        // Assert
        // An empty alt="" tells screen readers the image is decoration and can be skipped.
        // Leaving alt off entirely is different: some screen readers then read out the file name.
        // Listing one value per card stops the test passing when no images are found at all.
        html.QuerySelectorAll(".dfe-card img")
            .Select(image => image.GetAttribute("alt"))
            .ShouldBe(["", "", ""]);
    }

    /// <summary>
    /// Renders the cards. The component has no model.
    /// </summary>
    private Task<IDocument> RenderCardsAsync()
    {
        return renderer.RenderAsync<object?>(ViewPath, null);
    }
}
