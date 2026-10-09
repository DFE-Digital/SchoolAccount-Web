using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using SchoolAccount.Web.Mvc.Helpers;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Helpers;

public class UrlHelperExtensionsTests
{
    private readonly UrlHelper _url = new(
        new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor())
    );

    [Theory]
    [InlineData("/Dashboard")]
    [InlineData("/Journey?census=1")]
    public void A_local_return_url_is_kept(string returnUrl)
    {
        // Act
        var safeReturnUrl = _url.SafeReturnUrl(returnUrl);

        // Assert
        safeReturnUrl.IsAbsoluteUri.ShouldBeFalse();
        safeReturnUrl.OriginalString.ShouldBe(returnUrl);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("https://www.example.com/")]
    [InlineData("//www.example.com/")]
    [InlineData("/\\www.example.com/")]
    public void A_missing_or_external_return_url_becomes_the_home_page(string? returnUrl)
    {
        // Act
        var safeReturnUrl = _url.SafeReturnUrl(returnUrl);

        // Assert
        safeReturnUrl.OriginalString.ShouldBe("/");
    }
}
