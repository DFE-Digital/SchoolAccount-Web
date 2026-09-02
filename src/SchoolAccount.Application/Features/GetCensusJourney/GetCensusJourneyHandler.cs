using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Features.GetCensusJourney;

public class GetCensusJourneyHandler(ICollectApiClient collectApiClient)
    : IQueryHandler<GetCensusJourneyQuery, GetCensusJourneyResponse>
{
    public async Task<Result<GetCensusJourneyResponse>> Handle(
        GetCensusJourneyQuery query,
        CancellationToken cancellationToken
    )
    {
        var result = await collectApiClient.GetCensusJourneyContent(
            query.Id,
            query.EmailAddress,
            query.Organisations,
            cancellationToken
        );

        return await Task.FromResult(result);
    }
}
