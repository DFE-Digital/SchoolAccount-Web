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

        var result = response.FirstOrDefault(x => x.Interesting);

        return await Task.FromResult(result);
    }
}
