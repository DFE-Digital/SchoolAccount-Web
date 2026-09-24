using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.TestCommon.Builders.GetCensusJourney;

public class GetCensusJourneyContentResponseBuilder
{
    private readonly List<ImportantDate> _importantDates = [];
    private readonly List<UnderstandStatus> _understandStatuses = [];
    private readonly List<StepByStep> _steps = [];
    private string _callToActionLabel = "Go to Autumn Census 2026";

    private Uri _callToActionUrl = new(
        "https://www.gov.uk/guidance/complete-the-school-census/generate-and-submit-your-return"
    );

    private string _caption = "Complete your census return";
    private string _overview;
    private string _status = "Not Started";
    private string _title = "Autumn School Census";

    public static GetCensusJourneyContentResponseBuilder ACensusJourneyContentResponse()
    {
        return new GetCensusJourneyContentResponseBuilder();
    }

    public GetCensusJourneyContentResponseBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public GetCensusJourneyContentResponseBuilder WithCaption(string caption)
    {
        _caption = caption;
        return this;
    }

    public GetCensusJourneyContentResponseBuilder WithOverview(string overview)
    {
        _overview = overview;
        return this;
    }

    public GetCensusJourneyContentResponseBuilder WithStatus(string status)
    {
        _status = status;
        return this;
    }

    public GetCensusJourneyContentResponseBuilder WithImportantDate(
        GetCensusJourneyResponseImportantDateBuilder builder
    )
    {
        _importantDates.Add(builder.Build());
        return this;
    }

    public GetCensusJourneyContentResponseBuilder WithImportantDates(
        params GetCensusJourneyResponseImportantDateBuilder[] builders
    )
    {
        foreach (var builder in builders)
        {
            _importantDates.Add(builder.Build());
        }

        return this;
    }

    public GetCensusJourneyContentResponseBuilder WithUnderstandStatus(
        GetCensusJourneyResponseUnderstandStatusBuilder builder
    )
    {
        _understandStatuses.Add(builder.Build());
        return this;
    }

    public GetCensusJourneyContentResponseBuilder WithUnderstandStatuses(
        params GetCensusJourneyResponseUnderstandStatusBuilder[] builders
    )
    {
        foreach (var builder in builders)
        {
            _understandStatuses.Add(builder.Build());
        }

        return this;
    }

    public GetCensusJourneyContentResponseBuilder WithSteps()
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

    public GetCensusJourneyContentResponseBuilder WithCallToActionUrl(Uri callToActionUrl)
    {
        _callToActionUrl = callToActionUrl;
        return this;
    }

    public GetCensusJourneyContentResponseBuilder WithCallToActionLabel(
        string callToActionButtonText
    )
    {
        _callToActionLabel = callToActionButtonText;
        return this;
    }

    public Result<GetCensusJourneyContentResponse> AsSuccess()
    {
        return Result.Success(Build());
    }

    public GetCensusJourneyContentResponse Build()
    {
        return new GetCensusJourneyContentResponse
        {
            Title = _title,
            Caption = _caption,
            Overview = _overview,
            Status = _status,
            ImportantDates = _importantDates,
            UnderstandStatuses = _understandStatuses,
            StepByStep = _steps,
            CallToAction = new CallToAction { Label = _callToActionLabel, Url = _callToActionUrl },
        };
    }
}
