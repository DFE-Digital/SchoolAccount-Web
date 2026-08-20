using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Collect.CensusStatus;

public class GetCensusStatusHandler(ICollectApiService collectApiService)
    : IQueryHandler<GetCensusStatusQuery, GetCensusStatusResponse>
{
    public async Task<Result<GetCensusStatusResponse>> Handle(
        GetCensusStatusQuery query,
        CancellationToken cancellationToken
    )
    {
        var response = await collectApiService.GetCensusStatus(query);

        return await Task.FromResult(response);
    }
}
