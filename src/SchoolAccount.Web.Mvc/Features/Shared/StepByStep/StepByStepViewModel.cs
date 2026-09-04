using Microsoft.AspNetCore.Html;

namespace SchoolAccount.Web.Mvc.Features.Shared.StepByStep;

public class StepByStepViewModel(int index)
{
    public int Index { get; init; } = index;
    public string Title { get; init; }
    public IHtmlContent Content { get; init; }
    public StepByStepStatusViewModel? Status { get; init; }
    public bool IsOpen { get; set; }

    public bool TryGetStatus(out StepByStepStatusViewModel status)
    {
        status = Status!;
        return Status is not null;
    }
}
