using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Collect.CensusStatus;

public class GetCensusStatusHandler(ICollectApiService collectApiService)
    : IQueryHandler<GetCensusStatusQuery, List<GetCensusStatusResponse>>
{
    public async Task<Result<List<GetCensusStatusResponse>>> Handle(
        GetCensusStatusQuery query,
        CancellationToken cancellationToken
    )
    {
        var result = await collectApiService.GetCensusStatus(query);

        return result;
    }
}
