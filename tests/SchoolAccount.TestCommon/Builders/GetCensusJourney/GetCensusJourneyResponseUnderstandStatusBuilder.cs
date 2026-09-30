using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;

namespace SchoolAccount.TestCommon.Builders.GetCensusJourney;

public class GetCensusJourneyResponseUnderstandStatusBuilder
{
    private string _description = "Nothing has been uploaded";
    private string _name = "No Data";

    public static GetCensusJourneyResponseUnderstandStatusBuilder AUnderstandStatus()
    {
        return new GetCensusJourneyResponseUnderstandStatusBuilder();
    }

    public GetCensusJourneyResponseUnderstandStatusBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public GetCensusJourneyResponseUnderstandStatusBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public UnderstandStatus Build()
    {
        return new UnderstandStatus { Name = _name, Description = _description };
    }
}
