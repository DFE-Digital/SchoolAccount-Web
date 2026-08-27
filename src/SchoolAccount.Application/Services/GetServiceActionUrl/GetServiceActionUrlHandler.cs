using SchoolAccount.Application.Abstractions.Messaging;

namespace SchoolAccount.Application.Services.GetServiceActionUrl;

public class GetServiceActionUrlHandler()
    : IQueryHandler<GetServiceActionUrlQuery, GetServiceActionUrlResponse>
{
    public async Task<GetServiceActionUrlResponse> Handle(
        GetServiceActionUrlQuery query,
        CancellationToken cancellationToken
    ) { }
}
