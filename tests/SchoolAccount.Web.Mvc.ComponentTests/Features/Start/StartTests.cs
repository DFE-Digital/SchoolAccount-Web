using AngleSharp.Dom;
using SchoolAccount.Web.Mvc.ComponentTests.Common;
using SchoolAccount.Web.Mvc.Features.Start;
using Shouldly;

namespace SchoolAccount.Web.Mvc.ComponentTests.Features.Start;

public sealed class StartTests(ComponentRenderer renderer) : IClassFixture<ComponentRenderer>
{
    private const string ViewPath = "~/Features/Start/Start.cshtml";

    [Fact]
    public async Task Heading_invites_the_user_to_sign_in()
    {
        using var html = await RenderStartPageAsync();

        html.QuerySelector("h1")
            .ShouldNotBeNull()
            .TextContent.Trim()
            .ShouldBe("Sign in to School Account");
    }

    [Fact]
    public async Task Start_button_links_to_sign_in()
    {
        using var html = await RenderStartPageAsync();

        var startButton = html.QuerySelector("a.govuk-button--start").ShouldNotBeNull();
        startButton.GetAttribute("href").ShouldBe("/Account/Login");
        startButton.TextContent.Trim().ShouldBe("Sign in or create an account");
    }

    [Fact]
    public async Task Start_button_arrow_is_hidden_from_screen_readers()
    {
        using var html = await RenderStartPageAsync();

        var arrow = html.QuerySelector("a.govuk-button--start svg").ShouldNotBeNull();
        arrow.GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact]
    public async Task User_is_told_they_need_a_dfe_sign_in_account()
    {
        using var html = await RenderStartPageAsync();

        html.Body.ShouldNotBeNull()
            .TextContent.ShouldContain("You'll need a DfE Sign-in account.");
    }

    [Fact]
    public async Task Start_page_includes_the_benefit_cards()
    {
        using var html = await RenderStartPageAsync();

        html.QuerySelectorAll(".dfe-card").Length.ShouldBe(3);
    }

    private Task<IDocument> RenderStartPageAsync()
    {
        return renderer.RenderAsync(ViewPath, new StartViewModel());
    }
}
