using SchoolAccount.Application.Features.GetCensusJourney;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.TestCommon.Builders;

public class CensusJourneyResponseBuilder
{
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

    private GetCensusJourneyResponse Build()
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
