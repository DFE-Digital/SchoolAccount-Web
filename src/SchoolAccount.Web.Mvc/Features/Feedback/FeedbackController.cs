using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Features.Feedback;

public class FeedbackController(
    IUserContext userContext,
    ICommandHandler<FeedbackCommand> feedbackQueryHandler
) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(string feedbackMessage)
    {
        await feedbackQueryHandler.Handle(
            new FeedbackCommand
            {
                Message = feedbackMessage,
                Ukprn = userContext.Organisation?.Ukprn ?? "",
                Laestab =
                    userContext.Organisation?.LocalAuthority?.Code
                    + userContext.Organisation?.EstablishmentNumber,
            },
            CancellationToken.None
        );

        var model = new FeedbackViewModel { State = FeedbackState.Submitted };

        return PartialView("~/Features/Feedback/_Feedback.cshtml", model);
    }

    [HttpGet]
    public IActionResult Open()
    {
        var model = new FeedbackViewModel { State = FeedbackState.Responding };

        return PartialView("~/Features/Feedback/_Feedback.cshtml", model);
    }

    [HttpGet]
    public IActionResult Cancel()
    {
        var model = new FeedbackViewModel { State = FeedbackState.Initial };

        return PartialView("~/Features/Feedback/_Feedback.cshtml", model);
    }
}
