using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Collect.CensusStatuses;

public class GetCensusStatusesHandler(ICollectApiClient collectApiClient)
    : IQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>>
{
    public async Task<Result<List<GetCensusStatusesResponse>>> Handle(
        GetCensusStatusesQuery query,
        CancellationToken cancellationToken
    )
    {
        var result = await collectApiClient.GetCensusStatuses(
            query.Id,
            query.EmailAddress,
            query.Organisations,
            cancellationToken
        );

        return result;
    }
}
