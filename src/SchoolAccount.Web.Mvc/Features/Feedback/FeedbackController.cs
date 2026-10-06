using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Features.Feedback;

public class FeedbackController(
    IUserContext userContext,
    ICommandHandler<FeedbackCommand> feedbackCommandHandler
) : Controller
{
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(
        string feedbackMessage,
        string? returnUrl,
        CancellationToken cancellationToken
    )
    {
        var result = await feedbackCommandHandler.Handle(
            new FeedbackCommand
            {
                Message = feedbackMessage,
                Ukprn = userContext.Organisation?.Ukprn,
                OrganisationId = userContext.Organisation?.Id ?? string.Empty,
            },
            cancellationToken
        );

        if (result.IsSuccess)
        {
            TempData["FeedbackSubmitted"] = true;
        }

        if (!Url.IsLocalUrl(returnUrl))
        {
            returnUrl = "/";
        }

        return LocalRedirect(returnUrl + "#feedback-thanks");
    }
}
