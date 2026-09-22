using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Features.Feedback;

[ApiController]
public class FeedbackController : ControllerBase
{
    private const string FeedbackSubmittedName = "footer-feedback__row--submitted";

    [HttpPost]
    public IActionResult Respond() { }

    public IActionResult Submit() { }

    public IActionResult Cancel() { }
}
