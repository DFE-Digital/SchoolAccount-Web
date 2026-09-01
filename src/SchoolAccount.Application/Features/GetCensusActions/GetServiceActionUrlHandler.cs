using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Features.GetCensusActions;

public class GetServiceActionUrlHandler(ICollectApiClient collectApiClient)
    : IQueryHandler<GetServiceActionsQuery, GetServiceActionsResponse>
{
    public async Task<Result<GetServiceActionsResponse>> Handle(
        GetServiceActionsQuery query,
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
