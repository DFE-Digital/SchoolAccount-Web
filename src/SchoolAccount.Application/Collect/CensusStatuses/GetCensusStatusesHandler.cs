using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Collect.CensusStatuses;

public class GetCensusStatusesHandler(ICollectApiService collectApiService)
    : IQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>>
{
    public async Task<Result<List<GetCensusStatusesResponse>>> Handle(
        GetCensusStatusesQuery query,
        CancellationToken cancellationToken
    )
    {
        var result = await collectApiService.GetCensusStatuses(query, cancellationToken);

        return result;
    }
}
