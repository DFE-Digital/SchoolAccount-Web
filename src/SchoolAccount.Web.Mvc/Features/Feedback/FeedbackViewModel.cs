using Microsoft.AspNetCore.Mvc;

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
}

public class FeedbackViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var model = new FeedbackViewModel { State = FeedbackState.Initial };

        return View("~/Features/Feedback/_Feedback.cshtml", model);
    }
}
