using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Helpers;
using SchoolAccount.Web.Mvc.Infrastructure;

namespace SchoolAccount.Web.Mvc.Features.Feedback;

[FeatureGate(FeatureFlags.FeedbackFooter)]
public class FeedbackController(
    IUserContext userContext,
    ICommandHandler<FeedbackCommand> feedbackCommandHandler
) : Controller
{
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(FeedbackForm form, CancellationToken cancellationToken)
    {
        var returnUrl = Url.SafeReturnUrl(form.ReturnUrl);

        if (!ModelState.IsValid)
        {
            return RedirectBackWithErrors(returnUrl);
        }

        var result = await feedbackCommandHandler.Handle(
            new FeedbackCommand
            {
                Message = form.FeedbackMessage ?? string.Empty,
                Ukprn = userContext.Organisation?.Ukprn,
                OrganisationId = userContext.Organisation?.Id ?? string.Empty,
            },
            cancellationToken
        );

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(
                nameof(FeedbackForm.FeedbackMessage),
                "Sorry, there is a problem sending your feedback. Try again later"
            );

            return RedirectBackWithErrors(returnUrl);
        }

        TempData["FeedbackSubmitted"] = true;

        return LocalRedirect($"{returnUrl}#feedback-thanks");
    }

    [HttpGet]
    public IActionResult OpenFeedbackForm()
    {
        return View("~/Features/Feedback/NonJavascript/FeedbackForm.cshtml");
    }

    /// <summary>
    /// The form is on another page, so its errors go back there for the error summary.
    /// </summary>
    private LocalRedirectResult RedirectBackWithErrors(Uri returnUrl)
    {
        TempData["ErrorSummary"] = ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(entry => entry.Key, entry => entry.Value!.Errors[0].ErrorMessage);

        return LocalRedirect(returnUrl.ToString());
    }
}
