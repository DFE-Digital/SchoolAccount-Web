using SchoolAccount.Application.Features.GetCensusJourney;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.TestCommon.Builders.GetCensusJourney;

public class CensusJourneyResponseBuilder
{
    private string _title = "Autumn School Census";
    private string _caption = "Complete your census return";
    private string _overview = "";
    private List<GetCensusJourneyResponseImportantDates> _importantDates = [];
    private GetCensusJourneyResponseStatus _status = new GetCensusJourneyResponseStatus
    {
        Name = "notStarted",
        Label = "Not Started",
    };
    private string _callToActionLabel = "Go to Autumn Census 2026";
    private Uri _callToActionUrl = new(
        "https://www.gov.uk/guidance/complete-the-school-census/generate-and-submit-your-return"
    );

    public static CensusJourneyResponseBuilder Create() => new();

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

    public CensusJourneyResponseBuilder WithStatus(GetCensusJourneyResponseStatus status)
    {
        _status = status;
        return this;
    }

    public CensusJourneyResponseBuilder WithImportantDates(
        List<GetCensusJourneyResponseImportantDates> importantDates
    )
    {
        _importantDates = importantDates;
        return this;
    }

    public CensusJourneyResponseBuilder WithCallToActionUrl(Uri callToActionUrl)
    {
        _callToActionUrl = callToActionUrl;
        return this;
    }

    public CensusJourneyResponseBuilder WithCallToActionButtonText(string callToActionButtonText)
    {
        _callToActionLabel = callToActionButtonText;
        return this;
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
            CallToAction = new GetCensusJourneyResponseCallToAction
            {
                Label = _callToActionLabel,
                Url = _callToActionUrl,
            },
        };
    }

    public Result<GetCensusJourneyResponse> AsSuccess()
    {
        return Result.Success(Build());
    }
}
