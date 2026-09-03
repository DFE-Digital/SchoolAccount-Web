using SchoolAccount.Web.Mvc.Helpers;

namespace SchoolAccount.Web.Mvc.Features.Shared.StepByStep;

public class StepByStepViewModelCollection
{
    public Guid Identifier { get; init; }
    public List<StepByStepViewModel> Steps { get; } = [];

    protected StepByStepViewModelCollection(Guid identifier)
    {
        Identifier = identifier;
    }

    public static StepByStepViewModelCollection Create(Guid identifier)
    {
        return new StepByStepViewModelCollection(identifier);
    }

    protected StepByStepViewModelCollection(
        Guid identifier,
        IEnumerable<Application.Features.GetCensusJourney.StepByStep> steps
    )
        : this(identifier)
    {
        foreach (var step in steps)
        {
            AddStep(step);
        }
    }

    public static StepByStepViewModelCollection Create(
        Guid identifier,
        IEnumerable<Application.Features.GetCensusJourney.StepByStep> steps
    )
    {
        return new StepByStepViewModelCollection(identifier, steps);
    }

    public void AddStep(Application.Features.GetCensusJourney.StepByStep step)
    {
        Dictionary<string, string[]> requiredClasses = new()
        {
            ["p"] = ["app-step-nav__paragraph"],
            ["ul"] = ["govuk-list govuk-list--bullet"],
            ["a"] = ["govuk-link"],
        };

        var model = new StepByStepViewModel(Steps.Count + 1)
        {
            Title = step.Title,
            Content = HtmlContentHelper.AddClassesToNodes(step.Body, requiredClasses),
            IsOpen = step.IsOpen,
            Status = step.Status is not null
                ? new StepByStepStatusViewModel
                {
                    Label = step.Status.Label,
                    Colour = step.Status.Colour ?? "govuk-tag--grey",
                }
                : null,
        };

        Steps.Add(model);
    }

    public string DetermineWrapperIdentifier()
    {
        return $"sbs-{Identifier}-wrapper";
    }

    public string DetermineStepIdentifier(int index)
    {
        return $"sbs-{Identifier}-step-{index}";
    }

    public string DeterminePanelIdentifier(int index)
    {
        return $"sbs-{Identifier}-panel-{index}";
    }

    public bool HasItems()
    {
        return Steps.Count > 0;
    }
}
