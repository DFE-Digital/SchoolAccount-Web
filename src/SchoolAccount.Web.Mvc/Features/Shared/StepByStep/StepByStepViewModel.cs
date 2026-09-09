using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Html;

namespace SchoolAccount.Web.Mvc.Features.Shared.StepByStep;

public class StepByStepViewModel(int index)
{
    public int Index { get; init; } = index;

    public string Title { get; init; }

    public IHtmlContent Content { get; init; }

    public bool IsOpen { get; set; }

    public StepByStepStatusViewModel? Status { get; init; }

    [MemberNotNullWhen(true, nameof(Status))]
    public bool HasStatus => Status is not null;
}
