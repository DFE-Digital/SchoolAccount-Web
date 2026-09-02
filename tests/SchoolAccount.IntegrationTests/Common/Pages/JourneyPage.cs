using AngleSharp.Dom;

namespace SchoolAccount.IntegrationTests.Common.Pages;

/// <remarks>
/// This will be replaced with their own pages eventually
/// </remarks>
public class JourneyPage : AngleSharpPage
{
    public virtual JourneyStepsElement GetStepsComponent()
    {
        return new JourneyStepsElement(
            Page.QuerySelector($"div.app-step-nav[data-module='appstepnav']")
        );
    }

    public class JourneyStepsElement(IElement? wrapper)
    {
        public bool IsPresent => wrapper is not null;

        public List<JourneyStepElement> GetSteps()
        {
            return wrapper?.QuerySelectorAll("li").Select(x => new JourneyStepElement(x)).ToList()
                ?? [];
        }

        public class JourneyStepElement(IElement element)
        {
            public string? GetTitle()
            {
                return element.QuerySelector("span.js-step-title")?.TextContent;
            }
        }
    }
}
