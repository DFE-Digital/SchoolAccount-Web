using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc.Helpers;

namespace SchoolAccount.Web.Mvc.Features.Feedback;

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
            throw new InvalidOperationException("Feedback submission failed");
        }

        TempData["FeedbackSubmitted"] = true;

        return LocalRedirect($"{returnUrl}#feedback-thanks");
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
