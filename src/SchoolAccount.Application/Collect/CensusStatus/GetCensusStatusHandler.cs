using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Collect.CensusStatus;

public class GetCensusStatusHandler() : IQueryHandler<GetCensusStatusQuery, GetCensusStatusResponse>
{
    public async Task<Result<GetCensusStatusResponse>> Handle(
        GetCensusStatusQuery query,
        CancellationToken cancellationToken
    )
    {
        var response = new GetCensusStatusResponse()
        {
            Name = "Autumn School Census",
            Status = new Status() { Name = "Not started" },
        };

        return await Task.FromResult(response);
    }
}
