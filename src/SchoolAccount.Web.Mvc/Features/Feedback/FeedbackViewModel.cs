using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace SchoolAccount.Web.Mvc.Features.Feedback;

public enum FeedbackState
{
    Initial,
    Responding,
    Submitted,
}

public class FeedbackViewModel
{
    public FeedbackState State { get; init; }
    public string ReturnUrl { get; init; } = "/"; // current page, without ?feedback
    public string OpenUrl { get; init; } = "/"; // current page, with ?feedback=open
}

public class FeedbackViewComponent : ViewComponent
{
    public const string SubmittedKey = "FeedbackSubmitted";

    public IViewComponentResult Invoke()
    {
        var request = HttpContext.Request;

        // Current URL, keeping any other query params but dropping "feedback"
        var otherParams = request.Query.Where(q =>
            !string.Equals(q.Key, "feedback", StringComparison.OrdinalIgnoreCase)
        );
        var returnUrl = QueryHelpers.AddQueryString(
            $"{request.PathBase}{request.Path}",
            otherParams
        );
        var openUrl = QueryHelpers.AddQueryString(returnUrl, "feedback", "open");

        var submitted = TempData[SubmittedKey] as bool? == true;
        var responding = request.Query["feedback"] == "open";

        var state = FeedbackState.Initial;

        if (responding)
        {
            state = FeedbackState.Responding;
        }

        if (submitted)
        {
            state = FeedbackState.Submitted;
        }

        var model = new FeedbackViewModel
        {
            State = state,
            ReturnUrl = returnUrl,
            OpenUrl = openUrl,
        };

        return View("~/Features/Feedback/Feedback.cshtml", model);
    }
}
