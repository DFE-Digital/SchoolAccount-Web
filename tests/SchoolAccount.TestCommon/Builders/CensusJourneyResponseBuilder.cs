using SchoolAccount.Application.Features.GetCensusJourney;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.TestCommon.Builders;

public class CensusJourneyResponseBuilder
{
    private readonly List<StepByStep> _steps = [];
    private string _callToActionButtonText = "Go to Autumn Census 2026";
    private Uri _callToActionUrl = new(
        "https://www.gov.uk/guidance/complete-the-school-census/generate-and-submit-your-return"
    );

    public static CensusJourneyResponseBuilder Create() => new();

    public CensusJourneyResponseBuilder WithCallToActionUrl(Uri callToActionUrl)
    {
        _callToActionUrl = callToActionUrl;
        return this;
    }

    public CensusJourneyResponseBuilder WithCallToActionButtonText(string callToActionButtonText)
    {
        _callToActionButtonText = callToActionButtonText;
        return this;
    }

    public CensusJourneyResponseBuilder WithSteps()
    {
        _steps.Add(
            new StepByStep
            {
                Title = "This is a fake step 1",
                Body = "<p>This is a fake step body</p>",
            }
        );
        _steps.Add(
            new StepByStep
            {
                Title = "This is a fake step 2",
                Body = "<p>This is a super fake step body</p>",
            }
        );

        return this;
    }

    public CensusJourneyResponseBuilder WithStep(params StepByStep[] steps)
    {
        _steps.AddRange(steps);
        return this;
    }

    private GetCensusJourneyResponse Build()
    {
        return new GetCensusJourneyResponse
        {
            StepByStep = new StepByStepCollection(_steps),
            CallToAction = new CallToAction
            {
                ButtonText = _callToActionButtonText,
                Url = _callToActionUrl,
            },
        };
    }

    public Result<GetCensusJourneyResponse> AsSuccess()
    {
        return Result.Success(Build());
    }
}
