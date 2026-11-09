using SchoolAccount.Web.Mvc.Features.Error;

namespace SchoolAccount.IntegrationTests.Common.Pages;

public class ErrorPage : AngleSharpPage
{
    public bool IsForbiddenPageTitle()
    {
        var pageTitle = GetPageTitle();

        return pageTitle?.Equals(ErrorViewModel.ForbiddenTitle, StringComparison.Ordinal) == true;
    }

    public bool IsForbiddenPageHeading()
    {
        var pageHeading = GetFirstHeading();

        return pageHeading?.Equals("Access denied", StringComparison.Ordinal) == true;
    }

    public bool IsNotFoundPageTitle()
    {
        var pageTitle = GetPageTitle();

        return pageTitle?.Equals(ErrorViewModel.NotFoundTitle, StringComparison.Ordinal) == true;
    }

    public bool IsNotFoundPageHeading()
    {
        var pageHeading = GetFirstHeading();

        return pageHeading?.Equals("Page not found", StringComparison.Ordinal) == true;
    }

    public bool IsServerErrorPageTitle()
    {
        var pageTitle = GetPageTitle();

        return pageTitle?.Equals(ErrorViewModel.ErrorTitle, StringComparison.Ordinal) == true;
    }
}
