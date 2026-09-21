using SchoolAccount.Application.Features.Collect.CensusStatuses;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.TestCommon.Builders.GetCensusJourney;

public class GetCensusJourneyResponseBuilder
{
    private readonly List<GetCensusStatusesResponse> _schoolStatuses = [];
    private GetCensusJourneyContentResponse _content = GetCensusJourneyContentResponseBuilder
        .ACensusJourneyContentResponse()
        .Build();

    public static GetCensusJourneyResponseBuilder AGetCensusJourneyResponse()
    {
        return new GetCensusJourneyResponseBuilder();
    }

    public GetCensusJourneyResponseBuilder WithContent(
        GetCensusJourneyContentResponseBuilder builder
    )
    {
        _content = builder.Build();
        return this;
    }

    public GetCensusJourneyResponseBuilder WithSchoolStatus(CensusStatusesResponseBuilder builder)
    {
        _schoolStatuses.Add(builder.Build());
        return this;
    }

    public GetCensusJourneyResponseBuilder WithSchoolStatuses(
        params CensusStatusesResponseBuilder[] builders
    )
    {
        foreach (var builder in builders)
        {
            _schoolStatuses.Add(builder.Build());
        }

        return this;
    }

    public Result<GetCensusJourneyResponse> AsSuccess()
    {
        return Result.Success(Build());
    }

    public GetCensusJourneyResponse Build()
    {
        return new GetCensusJourneyResponse
        {
            Content = _content,
            SchoolStatuses = _schoolStatuses,
        };
    }
}
