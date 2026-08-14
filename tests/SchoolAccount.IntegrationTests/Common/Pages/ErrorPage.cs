using SchoolAccount.Web.Mvc.Features.Error;

namespace SchoolAccount.IntegrationTests.Common.Pages;

public class ErrorPage : AngleSharpPage
{
    public bool IsNotFoundPageTitle()
    {
        var pageTitle = GetTitle();

        return pageTitle?.Equals(ErrorViewModel.NotFoundTitle, StringComparison.Ordinal) == true;
    }

    public bool IsNotFoundPageHeading()
    {
        var pageHeading = GetFirstHeading();

        return pageHeading?.Equals("Page not found", StringComparison.Ordinal) == true;
    }

    public bool IsServerErrorPageTitle()
    {
        var pageTitle = GetTitle();

        return pageTitle?.Equals(ErrorViewModel.ErrorTitle, StringComparison.Ordinal) == true;
    }
}
