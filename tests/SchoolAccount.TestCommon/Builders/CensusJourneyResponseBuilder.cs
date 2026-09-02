using SchoolAccount.Application.Features.GetCensusJourney;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.TestCommon.Builders;

public class CensusJourneyResponseBuilder
{
    private string _callToActionButtonText = "Go to Autumn Census 2026";
    private string _callToActionUrl = "#";

    public static CensusJourneyResponseBuilder Create() => new();

    public CensusJourneyResponseBuilder WithCallToActionUrl(string callToActionUrl)
    {
        _callToActionUrl = callToActionUrl;
        return this;
    }

    public CensusJourneyResponseBuilder WithCallToActionButtonText(string callToActionButtonText)
    {
        _callToActionButtonText = callToActionButtonText;
        return this;
    }

    public GetCensusJourneyResponse Build()
    {
        return new GetCensusJourneyResponse
        {
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
