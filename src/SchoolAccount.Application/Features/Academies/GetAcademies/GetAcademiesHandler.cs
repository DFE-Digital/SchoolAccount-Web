using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Features.Academies.GetAcademies;

public class GetAcademiesHandler(IAcademiesApiClient academiesApiClient)
    : IQueryHandler<GetAcademiesQuery, GetAcademyTrustResponse>
{
    public async Task<Result<GetAcademyTrustResponse>> Handle(
        GetAcademiesQuery query,
        CancellationToken cancellationToken
    )
    {
        var result = await academiesApiClient.GetTrustDetails(query.Ukprn, cancellationToken);

        return result;
    }
}
