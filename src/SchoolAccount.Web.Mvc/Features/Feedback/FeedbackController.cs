using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.Web.Mvc.Features.Feedback;

public class FeedbackController : Controller
{
    [HttpGet]
    public IActionResult Feedback()
    {
        var model = new FeedbackViewModel { IsResponding = true };
        return View(model);
    }

    // public IActionResult Submit() { }

    [HttpGet]
    public IActionResult Cancel()
    {
        return View(new FeedbackViewModel());
    }
}
