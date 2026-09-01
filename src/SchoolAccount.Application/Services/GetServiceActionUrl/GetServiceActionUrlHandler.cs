using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Services.GetServiceActionUrl;

public class GetServiceActionUrlHandler()
    : IQueryHandler<GetServiceActionsQuery, GetServiceActionsResponse>
{
    public async Task<Result<GetServiceActionsResponse>> Handle(
        GetServiceActionsQuery query,
        CancellationToken cancellationToken
    )
    {
        var result = new GetServiceActionsResponse
        {
            CallToActionUrl =
                "https://www.gov.uk/guidance/complete-the-school-census/generate-and-submit-your-return",
        };

        return await Task.FromResult(result);
    }
}
