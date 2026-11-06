using AngleSharp.Dom;
using SchoolAccount.Web.Mvc.ComponentTests.Common;
using Shouldly;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Start;

public sealed class StartCardsTests(ComponentRenderer renderer) : IClassFixture<ComponentRenderer>
{
    private const string ViewPath = "~/Features/Start/_StartCards.cshtml";

    [Fact]
    public async Task Cards_are_introduced_with_a_heading()
    {
        using var html = await RenderCardsAsync();

        html.QuerySelector("h2").ShouldNotBeNull().TextContent.Trim().ShouldBe("What you'll get");
    }

    [Fact]
    public async Task Each_card_describes_a_benefit_of_the_service()
    {
        using var html = await RenderCardsAsync();

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
        using var html = await RenderCardsAsync();

        // One value per card so the test fails if no images are found.
        html.QuerySelectorAll(".dfe-card img")
            .Select(image => image.GetAttribute("alt"))
            .ShouldBe(["", "", ""]);
    }

    private Task<IDocument> RenderCardsAsync()
    {
        return renderer.RenderAsync<object?>(ViewPath, null);
    }
}
