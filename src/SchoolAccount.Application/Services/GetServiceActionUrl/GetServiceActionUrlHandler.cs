using SchoolAccount.Application.Abstractions.Messaging;

namespace SchoolAccount.Application.Services.GetServiceActionUrl;

public class GetServiceActionUrlHandler()
    : IQueryHandler<GetServiceActionsQuery, GetServiceActionsResponse>
{
    public async Task<GetServiceActionsResponse> Handle(
        GetServiceActionsQuery query,
        CancellationToken cancellationToken
    ) { }
}
