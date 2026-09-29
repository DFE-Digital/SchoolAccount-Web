using Azure;
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

        return Ok(new { success = true });
    }
}
