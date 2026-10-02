namespace SchoolAccount.Web.Mvc.Features.Feedback;

public enum FeedbackState
{
    Initial,
    Submitted,
}

public class FeedbackViewModel
{
    public FeedbackState State { get; init; }
}
