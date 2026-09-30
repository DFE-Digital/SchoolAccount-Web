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
