using System.Net;

namespace SchoolAccount.Web.Mvc.Features.Error;

public class ErrorViewModel(HttpStatusCode statusCode)
{
    public const string NotFoundTitle = "Not Found";
    public const string ForbiddenTitle = "Forbidden";
    public const string ErrorTitle = "Error";

    public HttpStatusCode StatusCode { get; } = statusCode;

    public string Title => CalculateTitle();

    public bool IsNotFound => StatusCode == HttpStatusCode.NotFound;

    public bool IsForbidden => StatusCode == HttpStatusCode.Forbidden;

    private string CalculateTitle()
    {
        if (IsNotFound)
        {
            return NotFoundTitle;
        }
        if (IsForbidden)
        {
            return ForbiddenTitle;
        }

        return ErrorTitle;
    }
}
