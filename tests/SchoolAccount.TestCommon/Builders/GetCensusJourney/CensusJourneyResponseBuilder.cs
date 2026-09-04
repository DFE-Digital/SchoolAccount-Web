using SchoolAccount.Application.Features.GetCensusJourney;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.TestCommon.Builders.GetCensusJourney;

public class CensusJourneyResponseBuilder
{
    private string _callToActionLabel = "Go to Autumn Census 2026";

    private Uri _callToActionUrl = new(
        "https://www.gov.uk/guidance/complete-the-school-census/generate-and-submit-your-return"
    );

    private string _caption = "Complete your census return";
    private readonly List<ImportantDate> _importantDates = [];
    private string _overview;
    private string _status = "Not Started";
    private readonly List<StepByStep> _steps = [];
    private string _title = "Autumn School Census";

    public static CensusJourneyResponseBuilder ACensusJourneyResponse()
    {
        return new CensusJourneyResponseBuilder();
    }

    public CensusJourneyResponseBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public CensusJourneyResponseBuilder WithCaption(string caption)
    {
        _caption = caption;
        return this;
    }

    public CensusJourneyResponseBuilder WithOverview(string overview)
    {
        _overview = overview;
        return this;
    }

    public CensusJourneyResponseBuilder WithStatus(string status)
    {
        _status = status;
        return this;
    }

    public CensusJourneyResponseBuilder WithImportantDate(
        GetCensusJourneyResponseImportantDateBuilder builder
    )
    {
        _importantDates.Add(builder.Build());
        return this;
    }

    public CensusJourneyResponseBuilder WithImportantDates(
        params GetCensusJourneyResponseImportantDateBuilder[] builders
    )
    {
        foreach (var builder in builders)
        {
            _importantDates.Add(builder.Build());
        }

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

    public CensusJourneyResponseBuilder WithCallToActionUrl(Uri callToActionUrl)
    {
        _callToActionUrl = callToActionUrl;
        return this;
    }

    public CensusJourneyResponseBuilder WithCallToActionLabel(string callToActionButtonText)
    {
        _callToActionLabel = callToActionButtonText;
        return this;
    }

    public Result<GetCensusJourneyResponse> AsSuccess()
    {
        return Result.Success(Build());
    }

    private GetCensusJourneyResponse Build()
    {
        return new GetCensusJourneyResponse
        {
            Title = _title,
            Caption = _caption,
            Overview = _overview,
            Status = _status,
            ImportantDates = _importantDates,
            StepByStep = _steps,
            CallToAction = new CallToAction { Label = _callToActionLabel, Url = _callToActionUrl },
        };
    }
}
